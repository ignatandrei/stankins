using System;
using System.Threading.Tasks;
using StankinsInterfaces;
using StanskinsImplementation;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ReceiverAzureIoTHub
{
    public class ReceiverFromAzureIoTHub : IReceive
    {
        public string Name { set; get; }
        public string ConnectionStringEventHubCompatible { get; set; }
        public string EntityEventHubCompatible { get; set; }
        public int StartTimeInHours { get; set; }
        public string FileNameSerializeLastOffset { get; set; }
        public object MessageType { get; set; }

        public IRowReceive[] valuesRead { get; set; }
        public Dictionary<string, LastReceivedMessage> lastRowValues { get; set; }

        private EventHubClient eventHubClient;
        private List<RowRead> receivedRows;

        public ReceiverFromAzureIoTHub(string connectionStringEventHubCompatible, string entityEventHubCompatible, string fileNameSerializeLastOffset, string messageType, int startTimeInHours = -24)
        {
            this.ConnectionStringEventHubCompatible = connectionStringEventHubCompatible;
            this.EntityEventHubCompatible = entityEventHubCompatible;
            this.FileNameSerializeLastOffset = fileNameSerializeLastOffset;
            this.MessageType = messageType;
            this.StartTimeInHours = startTimeInHours;
        }

        public async Task LoadData()
        {
            //Deserialize last received message
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastOffset))
            {
                Dictionary<string, object> lastRowValuesAsObjects = sdf.GetDictionary();
                lastRowValues = new Dictionary<string, LastReceivedMessage>();
                foreach (var item in lastRowValuesAsObjects)
                {
                    LastReceivedMessage lastItem;
                    lastItem.EnqueuedOffset = Convert.ToInt64(((string)item.Value).Split(',')[0]);
                    lastItem.EnqueuedTimeUtc = Convert.ToDateTime(((string)item.Value).Split(',')[1]);
                    lastRowValues.Add(item.Key, lastItem);
                }
            }

            //Load
            receivedRows = new List<RowRead>();

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(this.ConnectionStringEventHubCompatible)
            {
                EntityPath = this.EntityEventHubCompatible
            };
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            var runTimeInformation = await eventHubClient.GetRuntimeInformationAsync();

            foreach (string partition in runTimeInformation.PartitionIds)
            {
                // It checks if lastRowValues contains for current partition last received message
                if( !lastRowValues.ContainsKey(partition) )
                {
                    LastReceivedMessage lastReceivedMessage;
                    lastReceivedMessage.EnqueuedTimeUtc = this.StartTimeInHours == 0 ? DateTime.Now.AddHours(-24) : DateTime.UtcNow.AddHours(this.StartTimeInHours);
                    lastReceivedMessage.EnqueuedOffset = -1; //Starting offset for EventHub. Note: For IoT Hub, starting offset is 0.
                    lastRowValues.Add(partition, lastReceivedMessage);
                }

                // Receiving
                var receiver = eventHubClient.CreateReceiver(PartitionReceiver.DefaultConsumerGroupName, partition, lastRowValues[partition].EnqueuedTimeUtc);
                await ReceiveMessagesFromDeviceAsync(receiver);
            }

            valuesRead = receivedRows.ToArray();

            //Serialize last received message
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastOffset))
            {
                Dictionary<string, object> lastRowValuesAsObjects = new Dictionary<string, object>();
                foreach(var item in lastRowValues)
                {
                    lastRowValuesAsObjects.Add(item.Key, $"{item.Value.EnqueuedOffset},{item.Value.EnqueuedTimeUtc}");
                }
                sdf.SetDictionary(lastRowValuesAsObjects);
            }
        }

        protected async Task ReceiveMessagesFromDeviceAsync(PartitionReceiver receiver)
        {
            while (true)
            {
                var eventData = /*await*/ receiver.ReceiveAsync(5, new TimeSpan(0, 0, 5)); //Timeout for receiving messages = 30 seconds

                int count = 0;
                if (eventData != null)
                {
                    var result = eventData.Result;
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            count++;

                            if(item.SystemProperties.EnqueuedTimeUtc >= lastRowValues[receiver.PartitionId].EnqueuedTimeUtc && Convert.ToInt64(item.SystemProperties.Offset) > lastRowValues[receiver.PartitionId].EnqueuedOffset)
                            {
                                if(item.Properties.ContainsKey("MessageType") && item.Properties.ContainsKey("UnicodeEncoding"))
                                {
                                    if (this.MessageType.Equals(item.Properties["MessageType"]))
                                    {
                                        //Assert ? item.Properties["UnicodeEncoding"] == "UTF8"
                                        string data = UnicodeEncoding.UTF8.GetString(item.Body.Array);
                                        List<Dictionary<string, object>> results = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);

                                        foreach(var r in results)
                                        {
                                            RowRead row = new RowRead();
                                            row.Values = r;
                                            receivedRows.Add(row);
                                        }

                                    }
                                }

                                LastReceivedMessage lastReceivedMessage;
                                lastReceivedMessage.EnqueuedTimeUtc = item.SystemProperties.EnqueuedTimeUtc;
                                lastReceivedMessage.EnqueuedOffset = Convert.ToInt64(item.SystemProperties.Offset);
                                lastRowValues[receiver.PartitionId] = lastReceivedMessage;
                            }
                        }
                    }
                }
                if (count == 0)
                {
                    break;
                }
            }
        }
    }
}
