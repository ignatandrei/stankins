using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace StanskinsImplementation
{
    public class RowReadHierarchical: RowRead, IRowReceiveHierarchical
    {
        static int idH = 1;
        public RowReadHierarchical([CallerMemberName]string receiverName = ""):base(receiverName)
        {
            ID = idH++;
        }

        public IRowReceiveHierarchical Parent { get ; set ; }
        public long ID { get; set; }
        
    }
    public class RowRead : IRowReceive
    {
        public RowRead([CallerMemberName]string receiverName="")
        {
            this.CommonData = new CommonData();
            Values = new Dictionary<string, object>();
            this.ReceiverName = ReceiverName;
            
        }
        public ICommonData CommonData { get; }
        public string UserName { get; }
        public Dictionary<string, object> Values { get; set; }

        public string AdditionalDetails { get; }

        public string ReceiverName { get; }
    }
}
