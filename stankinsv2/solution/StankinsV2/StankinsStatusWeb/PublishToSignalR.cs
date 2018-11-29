using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class PublishToSignalR : INotificationHandler<ResultWithData>
    {
        public PublishToSignalR(MonitorOptions opt)
        {
            Opt = opt;
        }

        public MonitorOptions Opt { get; }

        public async Task Handle(ResultWithData notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.AliveResult.Process);
        }
    }
}
