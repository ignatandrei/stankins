using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace Transformers
{
    public class TransformerApplyReceiver:ITransform
    {

        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public IReceive Receiver { get; set; }
        public string PropertyNameReceiver { get; }
        public string Key { get; }

        public TransformerApplyReceiver(IReceive receiver, string propertyNameReceiver, string key)
        {
            Receiver = receiver;
            PropertyNameReceiver = propertyNameReceiver;
            Key = key;
            string type = (receiver == null) ? "" : receiver.GetType().Name;
            this.Name = $"apply {key} => {type}.{propertyNameReceiver}";
        }
        public async Task Run()
        {
            var ret = new List<IRowReceive>();
            var prop = Receiver.GetType().GetProperty(PropertyNameReceiver);
            var failed = new List<IRow>();
            foreach(var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                    continue;
                var val = item.Values[Key];
                prop.SetValue(Receiver, val);
                Receiver.ClearValues();
                try
                {
                    await Receiver.LoadData();
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    //@class.Log(LogLevel.Error,0,$"first error for {val} in transformerApplyReceiver",ex,null);
                    failed.Add(item);
                    continue;
                }
                if(Receiver.valuesRead?.Length>0)
                    ret.AddRange(Receiver.valuesRead);
            }
            //second chance to load....
            foreach(var item in failed)
            {
                var val = item.Values[Key];
                prop.SetValue(Receiver, val);
                Receiver.ClearValues();
                try
                {
                    await Receiver.LoadData();
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    //@class.Log(LogLevel.Error,0,$"second error for {val} in transformerApplyReceiver",ex,null);
                   
                }
            }

            valuesTransformed = ret.ToArray();
        }
        }
}
