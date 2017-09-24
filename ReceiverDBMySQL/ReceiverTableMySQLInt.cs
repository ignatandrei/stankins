using MySql.Data.MySqlClient;
using ReceiverDB;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;


namespace ReceiverDBMySQL
{
    public class ReceiverTableMySQLInt : ReceiverTableMySQL<int>
    {
        public ReceiverTableMySQLInt(DBTableData<int, MySqlConnection> dtd) : base(dtd)
        {
        }
        public new async Task LoadData()
        {
            //LOG: before retrieving latest value
            var valSaved = tableData.lastValue;

            var cn = await tableData.GetConnection();

            using (var cmd = cn.CreateCommand())
            {
                string fields = String.Join(",", tableData.Fields);
                cmd.CommandText = $"select {fields} from {tableData.TableName}; ";

                if (!valSaved.Equals(default(int)))
                {
                    cmd.CommandText += $" where {tableData.FieldNameToMark} > @lastValue";
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@lastValue";
                    param.Value = tableData.lastValue;
                    cmd.Parameters.Add(param);
                }
                cmd.CommandText += $" order by {tableData.FieldNameToMark} asc";
                cmd.CommandText += MaxRowsToLoad();
                ReadTableData(cmd);
            }
        }
        protected async void ReadTableData(DbCommand cmd)
        {

            var reader = await cmd.ExecuteReaderAsync();

            //TODO: replace with datatable
            var nrFields = reader.FieldCount;
            Dictionary<int, string> FieldNameToMarks = new Dictionary<int, string>();
            for (int i = 0; i < nrFields; i++)
            {
                FieldNameToMarks.Add(i, reader.GetName(i));
            }
            var values = new List<RowRead>();

            RowRead valLoop = null;
            while (reader.Read())
            {
                valLoop = new RowRead();
                for (int i = 0; i < nrFields; i++)
                {
                    var val = reader.GetValue(i);
                    if (val != null && val == DBNull.Value)
                    {
                        val = null;
                    }
                    valLoop.Values.Add(FieldNameToMarks[i], val);
                }
                values.Add(valLoop);
            }

            valuesRead = values.ToArray();

        }

    }
}
