using System.Collections.Generic;
using System.Data;

namespace StankinsV2Interfaces
{
    public interface IMetadata
    {
        IList<ITable> Tables { get; set; }
        IList<IColumn> Columns { get; set; }
        IList<IRelation> Relations { get; set; }
        IList<IHistory> Histories { get; set; }
        int AddTable(DataTable dt, int id);
        void AssignNewId(ITable table, int newId);
        void RemoveTable(ITable table);
    }
}
