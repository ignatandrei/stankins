using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TranformerApplyReceiver:ITransform
    {

        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public IReceive Receiver { get; set; }
        public string PropertyNameReceiver { get; }
        public string Key { get; }

        public TranformerApplyReceiver(IReceive receiver, string propertyNameReceiver, string key)
        {
            Receiver = receiver;
            PropertyNameReceiver = propertyNameReceiver;
            Key = key;
        }
        public async Task Run()
        {
            var ret = new List<IRow>();
            var prop = Receiver.GetType().GetProperty(PropertyNameReceiver);
            foreach(var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                    continue;
                var val = item.Values[Key];
                prop.SetValue(Receiver, val);
                Receiver.ClearValues();
                await Receiver.LoadData();
                ret.AddRange(Receiver.valuesRead);
            }
        }
        }
}
