using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformerApplySender
    {

        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public ISend Sender { get; set; }
        public string PropertyNameSender { get; }
        public string Key { get; }

        public TransformerApplySender(ISend sender, string propertyNameSender, string key)
        {
            Sender = sender;
            PropertyNameSender = propertyNameSender;
            Key = key;
            string type = (sender == null) ? "" : sender.GetType().Name;
            this.Name = $"apply {key} => {type}.{propertyNameSender}";
        }
        public async Task Run()
        {
            var ret = new List<IRowReceive>();
            var prop = Sender.GetType().GetProperty(PropertyNameSender);
            var failed = new List<IRow>();
            foreach (var item in valuesRead)
            {
                if (!item.Values.ContainsKey(Key))
                    continue;
                var val = item.Values[Key];
                prop.SetValue(Sender, val);
                try
                {
                    await Sender.Send();
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    //@class.Log(LogLevel.Error,0,$"first error for {val} in transformerApplyReceiver",ex,null);
                    failed.Add(item);
                    continue;
                }
                valuesTransformed = valuesRead;
            }



            valuesTransformed = ret.ToArray();
        }
    }
}
