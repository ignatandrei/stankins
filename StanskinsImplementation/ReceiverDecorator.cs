using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace StanskinsImplementation
{
    public class ReceiverDecorator : IReceive
    {
        public ReceiverDecorator(IReceive receive)
        {
            Receive = receive;
        }
        public IRowReceive[] valuesRead { get
            {
                return Receive.valuesRead;
            }
            
        }

        public string Name
        {
            get
            {
                return Receive.Name;
            }
            set
            {
                Receive.Name = value;
            }
        }
        IReceive Receive;

        public async Task LoadData()
        {
            try
            {
                //@class.Log(LogLevel.Trace,0,$"start load data {Name}",null,null);
                await Receive.LoadData();
                //@class.Log(LogLevel.Trace,0,$"end load data {Name} records: {Receive.valuesRead?.Length}",null,null);
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                //@class.Log(LogLevel.Trace,0,$"end load data ERROR {Name} records: {Receive.valuesRead?.Length}",ex,null);
                throw;
            }

        }
    }
}
