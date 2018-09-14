using StankinsV2Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StankinsV2Objects
{
    public class MetadataTable: IMetadata
    {
        public MetadataTable()
        {
            Tables = new List<ITable>(1);
            Columns = new List<IColumn>();
            Relations = new List<IRelation>();
        }
        public int AddTable(string name)
        {
            var nr = Tables.Count;
            Tables.Add(new Table() { Id = nr , Name = name });
            return nr;
        }
        public int AddTable(DataTable dt)
        {
            var id = AddTable(dt.TableName);
            foreach (DataColumn dc in dt.Columns)
            {
                this.AddColumn(dc.ColumnName, id);
            }
            
            return id;
        }
        public void AddColumn(string name, int idTable)
        {
            int nr= Columns.Count;
            Columns.Add(new Column()
            {
                Id = nr + 1,
                Name = name,
                IDTable = idTable
            });

        }

        public void AssignNewId(ITable table, int newId)
        {
            var oldId = table.Id;
            table.Id = newId;
            Columns.Where(c => c.IDTable == oldId).ToList().ForEach(it => it.IDTable = newId);
            
        }

        public void RemoveTable(ITable table)
        {
            var idTable = table.Id;
            for (int i = Columns.Count - 1; i >= 0; i--)
            {
                var col = Columns[i];
                if (col.IDTable == idTable)
                    Columns.RemoveAt(i);

            }
            Tables.RemoveAt(idTable);
        }

        public IList<IColumn> Columns { get ; set ; }
        public IList<IRelation> Relations { get ; set ; }
        public IList<IHistory> Histories { get ; set ; }
        public IList<ITable> Tables { get ; set ; }
    }
}
