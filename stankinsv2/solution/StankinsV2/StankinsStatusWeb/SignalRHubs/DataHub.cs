using Microsoft.AspNetCore.SignalR;
using StankinsStatusWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace StankinsAliveMonitor.SignalRHubs
{
    public interface ICommunication
    {
        Task SendMessageToClients(ResultWithData item);
    }
    public class DataHub:Hub<ICommunication>
    {
        private readonly ReplaySubject<ResultWithData> subject;

        public DataHub(ReplaySubject<ResultWithData> subject)
        {
            this.subject = subject;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            subject.Subscribe((d) =>
            {
                try
                {
                    this.Clients.Client(this.Context.ConnectionId).SendMessageToClients(d);
                }
                catch(Exception ex)
                {
                    //TODO: log
                }
            });
        }
        
        public async Task SendMessage(ResultWithData item)
        {
            await Clients.All.SendMessageToClients(item);
        }
    }
}
