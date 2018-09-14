using StankinsInterfaces;
using System.Runtime.CompilerServices;

namespace StanskinsImplementation
{
    public class RowReadHierarchical: RowRead, IRowReceiveHierarchicalParent
    {
        static long idH = 1;
        public RowReadHierarchical([CallerMemberName]string receiverName = ""):base(receiverName)
        {
            ID = idH++;
        }

        public IRowReceiveHierarchicalParent Parent { get ; set ; }
        public long ID { get; set; }
        
    }
}
