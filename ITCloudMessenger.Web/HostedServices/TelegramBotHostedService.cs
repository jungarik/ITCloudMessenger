using ITCloudMessenger.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
//using System.Timers;

namespace ITCloudMessenger.Web.HostedServices
{
    public class TelegramBotHostedService : IHostedService, IDisposable
    {
        private Timer _timer;

        IHubContext<MessengerHub> _hubcontext;

        TelegramBotClient botClient = new TelegramBotClient("781807727:AAEH23XIjUEHwQUMVG_iQ5mmQwC9vuCoKMg");

        int lastMsgId = 0;

        public TelegramBotHostedService(IHubContext<MessengerHub> hubcontext)
        {
            _hubcontext = hubcontext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
                var updates = botClient.GetUpdatesAsync().Result;
                var message = updates
                    .OrderByDescending(e => e.Message.Date)
                    .Select(e => e.Message)
                    .FirstOrDefault();

                if (message.MessageId != lastMsgId && !string.IsNullOrEmpty(message.Text))
                {
                    lastMsgId = message.MessageId;
                    _hubcontext
                        .Clients
                        .All
                        .SendAsync("ReceiveMessage", message.From.FirstName, message.Text);
                }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
