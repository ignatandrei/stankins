using AdysTech.InfluxDB.Client.Net;
//using InfluxDB.Collector;
//using InfluxDB.Collector.Diagnostics;
//using InfluxDB.LineProtocol.Client;
//using InfluxDB.LineProtocol.Payload;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SenderDBInflux
{
    public class SenderDB_Influx : ISend
    {
        public string Name { get; set; }
        public string url { get; set; }

        public IRow[] valuesToBeSent { set; private get; }
        public string db { get; set; }
        public string MeasurementName { get; set; }
        public string[] Fields { get; set; }
        public string[] Tags { get; set; }
        public SenderDB_Influx(
            string url, string db,string measurementName,
            string[] Fields, string[] Tags)
            
        {
            this.url = url;
            this.db = db;
            this.MeasurementName = measurementName;
            this.Fields = Fields;
            this.Tags = Tags;
            //Metrics.Collector = new CollectorConfiguration()
            //    .Tag.With("host", Environment.MachineName)
            //    //.Batch.AtInterval(TimeSpan.FromSeconds(2))
            //    .WriteTo.InfluxDB(url, db)
            //    .CreateCollector();

            //CollectorLog.RegisterErrorHandler(Errhandler);
        }
        static void Errhandler(string message , Exception exception)
        {
            //TODO: better logging
            Console.WriteLine($"{message}: {exception}");
        }

        public async Task Send()
        {
            InfluxDBClient client = new InfluxDBClient(url);
            var vals = new List<InfluxDatapoint<InfluxValueField>>();
            foreach (var item in valuesToBeSent)
            {
                var valMixed = new InfluxDatapoint<InfluxValueField>();
                valMixed.UtcTimestamp = DateTime.UtcNow;
                valMixed.Tags.Add("HOSTNAME", Environment.MachineName);
                valMixed.MeasurementName = MeasurementName;
                foreach (var fld in Fields)
                {

                    try
                    {
                        var val = (IComparable)item.Values[fld];
                        valMixed.Fields.Add(fld, new InfluxValueField(val));
                    }
                    catch (InvalidCastException ex)
                    {
                        //do nothing - field is not icomparable
                    }

                }
                foreach (var tag in Tags)
                {
                    var val = item.Values[tag];
                    valMixed.Tags.Add(tag, val?.ToString());
                }
                vals.Add(valMixed);


            }



            try
            {
                //TODO: log values to be sent
                if (vals.Count > 0)
                {
                    var r = await client.PostPointsAsync(db, vals);
                    await Task.Delay(1000);
                    if (!r)
                    {

                        //TODO: log error
                        Console.WriteLine("Errr");
                    }
                }
                else
                {
                    //TODO: Log no values to be sent
                }
            }
            catch (Exception ex)
            {
                //TODO: log
                string s = ex.Message;
                throw;
            }

        }
        
        static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
