using System.Collections.Generic;

namespace StankinsInterfaces
{
    public interface IRowReceiveRelation : IRowReceive
    {
        long ID { get; set; }
        Dictionary<string, List<IRowReceiveRelation>> Relations { get; set; }
    }
}