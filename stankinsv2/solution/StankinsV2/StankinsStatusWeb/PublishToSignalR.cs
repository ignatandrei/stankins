using MediatR;
using Microsoft.AspNetCore.SignalR;
using StankinsAliveMonitor.SignalRHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class PublishToSignalR : INotificationHandler<ResultWithData>
    {
        public PublishToSignalR(IHubContext<DataHub, ICommunication> opt)
        {
            Opt = opt;
        }

        public IHubContext<DataHub, ICommunication> Opt { get; }

        public async Task Handle(ResultWithData notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.AliveResult.Process);
            await Opt.Clients.All.SendMessageToClients(notification);
        }
    }

    public class PublishToSubjectReplay : INotificationHandler<ResultWithData>
    {
        private readonly ReplaySubject<ResultWithData> subject;

        public PublishToSubjectReplay(ReplaySubject<ResultWithData> subject)
        {
            this.subject = subject;
        }
        public async Task Handle(ResultWithData notification, CancellationToken cancellationToken)
        {
            subject.OnNext(notification);
        }
    }
}
