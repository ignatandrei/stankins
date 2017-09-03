using System;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Text;
using Newtonsoft.Json;

namespace SenderAzureIoTHub
{
    public class SenderToAzureIoTHub : ISend
    {
        public string IoTHubUri { get; set; }
        public string DeviceId { get; set; }
        public string DeviceKey { get; set; }
        public IRow[] valuesToBeSent { set; get; }
        public string Name { get; set; }

        public SenderToAzureIoTHub(string ioTHubUri, string deviceId, string deviceKey)
        {
            this.IoTHubUri = ioTHubUri;
            this.DeviceId = deviceId;
            this.DeviceKey = deviceKey;
        }

        public async Task Send()
        {
            var deviceClient = DeviceClient.Create(
                this.IoTHubUri,
                AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(this.DeviceId, this.DeviceKey),
                Microsoft.Azure.Devices.Client.TransportType.Http1
            );

            foreach (var row in this.valuesToBeSent)
            {
                string data = JsonConvert.SerializeObject(row.Values);
                var message = new Message(Encoding.Unicode.GetBytes(data)); 

                await deviceClient.SendEventAsync(message);
            }
        }
    }
}
