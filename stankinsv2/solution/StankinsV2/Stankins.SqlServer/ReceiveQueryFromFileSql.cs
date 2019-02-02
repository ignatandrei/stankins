using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Stankins.File;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SqlServer
{
    public class ReceiveQueryFromFileSql : BaseObjectInSerial<ReceiverReadFileText, TransformerOneTableToMulti< ReceiveQueryFromDatabaseSql>>, IReceive
    {
        public ReceiveQueryFromFileSql(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiveQueryFromFileSql);
        }

        public ReceiveQueryFromFileSql(string fileName, string connectionString) : this(new CtorDictionary()
        {
            {nameof(fileName), fileName},
            {nameof(connectionString),connectionString },
            {"receiverProperty","sql" },
            {"columnNameWithData","FileContents" },
            {"connectionType",typeof(SqlConnection).FullName }

        })
        {
            
        }
    }
}
