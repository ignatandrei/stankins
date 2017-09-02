using StankinsInterfaces;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace StanskinsImplementation
{
    public class RowReadRelation : RowRead, IRowReceiveRelation
    {
        static long idH = 1;
        public RowReadRelation([CallerMemberName]string receiverName = ""):base(receiverName)
        {
            ID = idH++;
            Relations = new Dictionary<string,List< IRowReceiveRelation>>();

        }
        public void Add(string name, IRowReceiveRelation rel)
        {
            if (!Relations.ContainsKey(name))
            {
                Relations.Add(name, new List<IRowReceiveRelation>());
            }
            Relations[name].Add(rel);
        }
        public long ID { get ; set ; }
        public Dictionary<string, List< IRowReceiveRelation>> Relations { get ; set ; }
    }
}
