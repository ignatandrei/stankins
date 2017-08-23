using System;
using System.Threading.Tasks;
using StankinsInterfaces;
using Transformers;
using System.ComponentModel;

namespace SenderFilter
{
    public class SenderWithFilter : ISend
    {
        public string Filter { get; set; } //Syntax: "DataType FieldName Operator ConstValue"  (ex. "System.Int32 ElapsedTime >= 3000")
        public ISend Sender { get; set; }
        public bool ExecuteSenderWhenEmptyData = false;
        public IRow[] valuesToBeSent { set; get; }

        public SenderWithFilter() { }

        public SenderWithFilter(string filter, ISend sender, bool executeSenderWhenEmptyData = false) {
            this.Filter = filter;
            this.Sender = sender;
            this.ExecuteSenderWhenEmptyData = executeSenderWhenEmptyData;
        }

        public async Task Send()
        {
            //Deserialize Filter into TransformerFilter_
            var filterAsObject = (FilterComparable)((TypeConverter)new FilterComparable()).ConvertFrom(this.Filter);
            filterAsObject.valuesRead = this.valuesToBeSent;
            //Apply filter
            await filterAsObject.Run();
            this.valuesToBeSent = filterAsObject.valuesTransformed;

            //Send data to real sender
            if(this.valuesToBeSent.Length > 0 || this.valuesToBeSent.Length == 0 && this.ExecuteSenderWhenEmptyData)
            {
                this.Sender.valuesToBeSent = this.valuesToBeSent;
                await this.Sender.Send();
            }
        }
    }
}
