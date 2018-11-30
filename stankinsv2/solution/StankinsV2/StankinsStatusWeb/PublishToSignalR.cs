using MediatR;
using Microsoft.AspNetCore.SignalR;
using StankinsAliveMonitor.SignalRHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class PublishToSignalR : INotificationHandler<ResultWithData>
    {
        public PublishToSignalR(IHubContext<DataHub> opt)
        {
            Opt = opt;
        }

        public IHubContext<DataHub> Opt { get; }

        public async Task Handle(ResultWithData notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.AliveResult.Process);
            await Opt.Clients.All.SendAsync("SendMessageToClients", notification);
        }
    }
}
