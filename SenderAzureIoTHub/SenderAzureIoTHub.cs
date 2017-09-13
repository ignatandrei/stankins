using System;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SenderAzureIoTHub
{
    public class SenderToAzureIoTHub : ISend
    {
        public string IoTHubUri { get; set; }
        public string DeviceId { get; set; }
        public string DeviceKey { get; set; }
        public IRow[] valuesToBeSent { set; get; }
        public string Name { get; set; }
        public string MessageType { get; set; }

        public SenderToAzureIoTHub(string ioTHubUri, string deviceId, string deviceKey, string messageType)
        {
            this.IoTHubUri = ioTHubUri;
            this.DeviceId = deviceId;
            this.DeviceKey = deviceKey;
            this.MessageType = messageType;
        }

        public async Task Send()
        {
            var deviceClient = DeviceClient.Create(
                this.IoTHubUri,
                AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(this.DeviceId, this.DeviceKey),
                Microsoft.Azure.Devices.Client.TransportType.Http1
            );

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (var row in this.valuesToBeSent)
            {
                rows.Add(row.Values);
            }
            string data = JsonConvert.SerializeObject(rows);
            var message = new Message(UnicodeEncoding.UTF8.GetBytes(data));
            message.Properties["MessageType"] = this.MessageType;
            message.Properties["UnicodeEncoding"] = "UTF8";

            await deviceClient.SendEventAsync(message);
        }
    }
}
