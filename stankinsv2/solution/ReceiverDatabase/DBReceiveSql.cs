using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ReceiverDatabase
{
    public class DBReceiveSql<Connection> : Receiver
    where Connection : DbConnection, new()
    {
        public DBReceiveSql(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(DBReceiveSql<Connection>);

            ConnectionString = base.GetMyDataOrThrow<string>(nameof(ConnectionString));
            var data = base.GetMyDataOrThrow<string>(nameof(CommandType));
            if (int.TryParse(data, out var val))
            {
                CommandType = (CommandType)val;
            }
            else
            {
                CommandType = (CommandType)Enum.Parse(typeof(CommandType), data);
            }

            CommandText = base.GetMyDataOrThrow<string>(nameof(CommandText));
        }

        

        /// <summary>
        /// TODO  : make a constructor with parameters from command
        /// </summary>        
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        public DBReceiveSql(string connectionString, CommandType commandType, string commandText)
            : this(new CtorDictionary()
            {
                {nameof(ConnectionString),connectionString },
                {nameof(commandType),commandType.ToString() },
                {nameof(commandText),commandText },
            })
        {

        }


        public string ConnectionString { get; }
        public CommandType CommandType { get; }
        public string CommandText { get; }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {            
            using (var con = new Connection())
            {
                con.ConnectionString = ConnectionString;
                await con.OpenAsync();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = this.CommandText;
                    cmd.CommandType = this.CommandType;
                    using (var ir = await cmd.ExecuteReaderAsync())
                    {
                        #region gather data
                        var dt = new DataTable
                        {
                            TableName = this.Name
                        };
                        dt.Load(ir);
                        #endregion

                        #region gather metadata
                        var ret = new DataToSentTable();
                        var id= ret.AddNewTable(dt);
                        ret.Metadata.AddTable(dt,id);
                        #endregion
                        return ret;
                    }


                }
            }

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
