using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StanskinsImplementation
{
    public class SendDecorator : ISend
    {
        public SendDecorator(ISend sendor)
        {
            Sendor = sendor;
        }
        public IRow[] valuesToBeSent
        {
            set
            {
                Sendor.valuesToBeSent = value;
            }
        }
        public string Name
        {
            get
            {
                return Sendor.Name;
            }
            set
            {
                Sendor.Name = value;
            }
        }
        ISend Sendor;

        public async Task Send()
        {
            //@class.Log(LogLevel.Trace,0,$"send data {Name} ",null,null);
            await Sendor.Send();
            //@class.Log(LogLevel.Trace,0,$"end send data {Name} ",null,null);

        }
    }
}
