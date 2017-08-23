using System;
using System.Threading.Tasks;
using StankinsInterfaces;
using Transformers;
using System.ComponentModel;

namespace SenderFilter
{
    public class SenderWithFilterComparable : ISend
    {
        public FilterComparable Transformer { get; set; }
        public ISend Sender { get; set; }
        public bool ExecuteSenderWhenEmptyData = false;
        public IRow[] valuesToBeSent { set; get; }

        public SenderWithFilterComparable() { }

        public SenderWithFilterComparable(FilterComparable transformer, ISend sender, bool executeSenderWhenEmptyData = false) {
            this.Transformer = transformer;
            this.Sender = sender;
            this.ExecuteSenderWhenEmptyData = executeSenderWhenEmptyData;
        }

        public async Task Send()
        {
            //First: Apply filter
            Transformer.valuesRead = this.valuesToBeSent;
            await Transformer.Run();
            this.valuesToBeSent = Transformer.valuesTransformed;

            //Second: Send data to real sender
            if(this.valuesToBeSent.Length > 0 || this.valuesToBeSent.Length == 0 && this.ExecuteSenderWhenEmptyData)
            {
                this.Sender.valuesToBeSent = this.valuesToBeSent;
                await this.Sender.Send();
            }
        }
    }
}
