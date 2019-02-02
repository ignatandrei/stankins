using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace StankinsReceiverDB
{
    public abstract class DatabaseReceiver: BaseObject, IReceive
    {
        protected readonly string connectionString;
        protected string connectionType;

        
        public DatabaseReceiver(CtorDictionary dict) : base(dict)
        {
            connectionString = GetMyDataOrThrow<string>(nameof(connectionString));
            connectionType = GetMyDataOrThrow<string>(nameof(connectionType));
        }
        public DatabaseReceiver(string connectionString, string connectionType) : this(new CtorDictionary()
        {
            {nameof(connectionString),connectionString },
            {nameof(connectionType),connectionType }
        })
        {

        }
        protected virtual DbConnection NewConnection()
        {
            var t = Type.GetType(connectionType);
            return Activator.CreateInstance(t) as DbConnection;
        }
        protected async Task<DataTable> FromSql(string stmtSql,string name)
        {
            using (var cn = NewConnection())
            {
                cn.ConnectionString = connectionString;
                await cn.OpenAsync();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = stmtSql;
                    cmd.CommandType = CommandType.Text;
                    using (var ir = await cmd.ExecuteReaderAsync())
                    {
                        var dt = new DataTable();                     
                        dt.Load(ir);
                        dt.TableName = name;
                        return dt;


                    }
                }
            }
        }
        protected async Task<DataTable> FromSqlToProperties(string stmtSql)
        {
            DataTable dt=new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("TableName", typeof(string));
            dt.Columns.Add("valueName", typeof(string));
            dt.Columns.Add("value", typeof(object));
            using (var cn = NewConnection())
            {
                cn.ConnectionString = connectionString;
                await cn.OpenAsync();
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = stmtSql;
                    cmd.CommandType = CommandType.Text;
                    using (var ir = await cmd.ExecuteReaderAsync())
                    {
                        var l = ir.FieldCount;
                        var names = new Dictionary<int,string>();
                        for (int i = 0; i < l; i++)
                        {
                            string name = ir.GetName(i);
                            if(name == "id" || name=="TableName")
                                continue;
                            names.Add(i,name);

                        }
                        while (await ir.ReadAsync())
                        {
                            var id = ir["id"].ToString();
                            var nameTable = ir["TableName"].ToString();
                            foreach (var name in names)
                            {
                                dt.Rows.Add(id, nameTable,  name.Value, ir[name.Key]);
                            }

                        }

                        return dt;

                    }
                }
            }
        }
    }
}
