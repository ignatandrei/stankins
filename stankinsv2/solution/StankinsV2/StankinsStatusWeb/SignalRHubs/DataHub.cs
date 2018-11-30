using Microsoft.AspNetCore.SignalR;
using StankinsStatusWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsAliveMonitor.SignalRHubs
{
    public interface ICommunication
    {
        Task SendMessageToClients(ResultWithData item);
    }
    public class DataHub:Hub<ICommunication>
    {

        public async Task SendMessage(ResultWithData item)
        {
            await Clients.All.SendMessageToClients(item);
        }
    }
}
