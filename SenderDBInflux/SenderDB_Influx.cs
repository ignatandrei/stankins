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
using Microsoft.Extensions.Logging;
namespace SenderDBInflux
{
    /// <summary>
    /// Send data to InfluxDB instance.
    /// </summary>
    public class SenderDB_Influx : ISend
    {
        /// <summary>
        /// Name of sender instance.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///The URL of InfluxDB instance.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Get or set the IRow object used to store values to be sent.
        /// </summary>
        public IRow[] valuesToBeSent { set; private get; }
        /// <summary>
        /// Get or set database name.
        /// </summary>
        public string db { get; set; }
        /// <summary>
        /// Get or set the measurement name.
        /// </summary>
        public string MeasurementName { get; set; }
        /// <summary>
        /// Get or set the array of fields.
        /// </summary>
        public string[] Fields { get; set; }
        /// <summary>
        /// Get or set the array of tags.
        /// </summary>
        public string[] Tags { get; set; }
        /// <summary>
        /// Initializes a new instance of the SenderDB_Influx class.
        /// </summary>
        /// <param name="url">See <see cref="url"/> property.</param>
        /// <param name="db">See <see cref="db"/> property.</param>
        /// <param name="measurementName">See <see cref="MeasurementName"/> property.</param>
        /// <param name="Fields">See <see cref="Fields"/> property.</param>
        /// <param name="Tags">See <see cref="Tags"/> property.</param>
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

        /// <summary>
        /// Executes the sender reading data from valuesToBeSent.
        /// </summary>
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

                        string message = "error influx post points";
                        //@class.Log(LogLevel.Error, 0, $"{message}", null, null);                        
                        message += "";
                    }
                }
                else
                {
                    string message = "no values to be sent";                    
                    //@class.Log(LogLevel.Information, 0, $"sender influx {message}", null, null);                        
                    message += "";
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //@class.Log(LogLevel.Error, 0, $"sender influx error {message}", ex, null);
                message += "";
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
