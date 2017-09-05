using CommonDB;
using StankinsInterfaces;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SenderBulkCopy
{
    public class SenderSqlServerBulkCopy :  ISend
    {
        public SenderSqlServerBulkCopy(DBTableDataConnection<SqlConnection> data) 
        {
            this.Name = "sender to sql server table by bulk copy";
            Data = data;
            Options = SqlBulkCopyOptions.Default;
        }
        public SqlBulkCopyOptions Options;
        public IRow[] valuesToBeSent { get; set; }
        public string Name { get; set; }
        public DBTableDataConnection<SqlConnection> Data { get; set; }

        public async Task Send()
        {
            if (Data?.Fields?.Length==0)
                throw new ArgumentException("please name the fields in Data");
            var rdr = new RowDataReader(valuesToBeSent, Data.Fields);
            var con = Data.ConnectionString;
            using (var sbc=new SqlBulkCopy(con, Options))
            {
                sbc.DestinationTableName = Data.TableName;

                await sbc.WriteToServerAsync(rdr);
            }
        }
    }
}
