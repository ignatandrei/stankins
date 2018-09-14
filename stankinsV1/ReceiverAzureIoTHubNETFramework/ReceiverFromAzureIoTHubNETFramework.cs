using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StankinsInterfaces;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using StanskinsImplementation;

namespace ReceiverAzureIoTHub
{
    public class ReceiverFromAzureIoTHubNETFramework : IReceive
    {
        public string Name { set; get; }
        public string ConnectionString { get; set; }
        public string Endpoint { get; set; }
        public string FileNameSerializeLastOffset { get; set; }
        public DateTime EnqueuedTimeUtc { get; set; }
        public long EnqueuedOffset { get; set; }
        public IRowReceive[] valuesRead { set; get; }
        private Dictionary<string, object> lastRowValues;

        public ReceiverFromAzureIoTHubNETFramework(string connectionString, string endpoint, string fileNameSerializeLastOffset)
        {
            this.ConnectionString = connectionString;
            this.Endpoint = endpoint;
            this.FileNameSerializeLastOffset = fileNameSerializeLastOffset;
        }

        public async Task LoadData()
        {
            //Deserialize last received message
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastOffset))
            {
                lastRowValues = sdf.GetDictionary();
            }
            this.EnqueuedTimeUtc = (lastRowValues.ContainsKey("EnqueuedTimeUtc") ? (DateTime)lastRowValues["EnqueuedTimeUtc"] : new DateTime(1,1,1));
            this.EnqueuedOffset = (lastRowValues.ContainsKey("EnqueuedOffset") ? (long)lastRowValues["EnqueuedOffset"] : 0);

            //Load
            var eventHubClient = EventHubClient.
                CreateFromConnectionString(this.ConnectionString, this.Endpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, this.EnqueuedTimeUtc);
                ReceiveMessagesFromDeviceAsync(receiver);
            }

            //Serialize last received message
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastOffset))
            {
                lastRowValues["EnqueuedTimeUtc"] = this.EnqueuedTimeUtc;
                lastRowValues["EnqueuedOffset"] = this.EnqueuedOffset;
                sdf.SetDictionary(lastRowValues);
            }
        }

        protected void ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                var eventData = receiver.Receive(10, new TimeSpan(0, 0, 10));
                if (eventData == null)
                {
                    break;
                }

                List<RowRead> receivedRows = new List<RowRead>();
                int count = 0;

                foreach (var ed in eventData)
                {
                    count++;
                    string data = Encoding.Unicode.GetString(ed.GetBytes());
                    RowRead row = new RowRead();
                    row.Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    receivedRows.Add(row);

                    this.EnqueuedTimeUtc = ed.EnqueuedTimeUtc;
                    this.EnqueuedOffset = long.Parse(ed.Offset);
                }
                if (count == 0)
                {
                    break;
                }
            }
        }
    }
}
