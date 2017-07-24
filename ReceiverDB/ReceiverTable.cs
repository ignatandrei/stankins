using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ReceiverDB
{
    //TODO: modify IEquatable with IComparable
    public abstract class ReceiverTable<T, Connection> : IReceive<T>
         where T : IEquatable<T>
         where Connection : DbConnection, new()
    {
        
        
        public DBTableData<T, Connection> tableData { get; set; }
        public IRowReceive[] valuesRead { get; private set; }
        
        public T LastValue
        {
            get
            {
                return tableData.lastValue;
            }
            set
            {
                tableData.lastValue = value;
            }
        }

        public ReceiverTable(DBTableData<T,Connection> dtd)
        {
           
            this.tableData = dtd;
        }

        public async Task LoadData()
        {
            //LOG: before retrieving latest value
            var valSaved = tableData.lastValue;

            var cn = await tableData.GetConnection();
            
            using (var cmd = cn.CreateCommand())
            {
                string fields = String.Join(",", tableData.Fields);
                cmd.CommandText = $"select {fields} from {tableData.TableName} ";

                if (!valSaved.Equals(default(T)))
                {
                    cmd.CommandText += $" where {tableData.FieldNameToMark} > @lastValue";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@lastValue";
                    param.Value = tableData.lastValue;
                    cmd.Parameters.Add(param);
                }
                cmd.CommandText += $" order by {tableData.FieldNameToMark} desc";
                var reader = await cmd.ExecuteReaderAsync();
                //TODO: replace with datatable
                var nrFields = reader.FieldCount;
                Dictionary<int, string> FieldNameToMarks = new Dictionary<int, string>();
                for (int i = 0; i < nrFields; i++)
                {
                    FieldNameToMarks.Add(i, reader.GetName(i));
                }
                var values = new List<RowRead>();
                bool first = true;
                while (await reader.ReadAsync())
                {
                    if (first)
                    {
                        var valRead = reader[tableData.FieldNameToMark];
                        if (valRead != null && valRead != DBNull.Value)
                        {
                            tableData.lastValue = (T)Convert.ChangeType(valRead, typeof(T));                            
                        }
                        first = false;

                    }
                    var valLoop = new RowRead();
                    for (int i = 0; i < nrFields; i++)
                    {
                        valLoop.Values.Add(FieldNameToMarks[i], reader.GetValue(i));
                    }
                    values.Add(valLoop);
                }
                valuesRead = values.ToArray();



            }
        }

       
    }
}
