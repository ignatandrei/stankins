using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ReceiverDB
{
    
    public abstract class ReceiverTable<T, Connection> : IReceive<T>
         where T : IComparable<T>
         where Connection : DbConnection, new()
    {

        public string Name { get; set; }
        public DBTableData<T, Connection> tableData { get; set; }
        public IRowReceive[] valuesRead { get; protected set; }
        
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
        protected virtual string MaxRowsToLoad()
        {
            return "";
        }
        public async Task LoadData()
        {
            //LOG: before retrieving latest value
            var valSaved = tableData.lastValue;

            var cn = await tableData.GetConnection();
            bool CompareValues = !String.IsNullOrWhiteSpace(tableData.FieldNameToMark);
            using (var cmd = cn.CreateCommand())
            {
                string fields = String.Join(",", tableData.Fields);
                string CommandText = $"select {fields} from {tableData.TableName} ";

                if (CompareValues && !valSaved.Equals(default(T)))
                {
                    CommandText += $" where {tableData.FieldNameToMark} > @lastValue";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@lastValue";
                    param.Value = tableData.lastValue;
                    cmd.Parameters.Add(param);
                }
                if (CompareValues)
                {
                    CommandText += $" order by {tableData.FieldNameToMark} asc";
                }
                CommandText += MaxRowsToLoad();
                cmd.CommandText = CommandText;
                var reader = await cmd.ExecuteReaderAsync();
                //TODO: replace with datatable
                var nrFields = reader.FieldCount;
                Dictionary<int, string> FieldNameToMarks = new Dictionary<int, string>();
                for (int i = 0; i < nrFields; i++)
                {
                    FieldNameToMarks.Add(i, reader.GetName(i));
                }
                var values = new List<RowRead>();
                
                RowRead valLoop=null;
                while (await reader.ReadAsync())
                {
                    
                    valLoop = new RowRead();
                    for (int i = 0; i < nrFields; i++)
                    {
                        var val = reader.GetValue(i);
                        //transform DBNull.Value into null
                        if (val != null && val == DBNull.Value)
                        {
                            val = null;
                        }
                        valLoop.Values.Add(FieldNameToMarks[i],val );
                    }
                    values.Add(valLoop);
                }

                valuesRead = values.ToArray();
                if (values.Count > 0 && CompareValues)
                {
                    
                    var lastVal = valLoop.Values[tableData.FieldNameToMark];
                    if (lastVal != null && lastVal != DBNull.Value)
                    {
                        tableData.lastValue = (T)Convert.ChangeType(lastVal, typeof(T));
                    }
                }


            }
        }

        public void ClearValues()
        {
            valuesRead = null;
        }
    }
}
