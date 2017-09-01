using System.Collections.Generic;

namespace StankinsInterfaces
{
    public interface IRowReceiveRelation : IRowReceive
    {
        long ID { get; set; }
        Dictionary<string, IRowReceiveRelation> Relations { get; set; }
    }
}