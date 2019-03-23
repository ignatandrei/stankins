using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Stankins.FileOps;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SqlServer
{
    public class ReceiveQueryFromFileSql : BaseObjectInSerial<ReceiverReadFileText,TransformSplitColumnAddRow,  TransformerOneTableToMulti< ReceiveQueryFromDatabaseSql>>, IReceive
    {
        public ReceiveQueryFromFileSql(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiveQueryFromFileSql);
        }

        public ReceiveQueryFromFileSql(string fileName, string connectionString) : this(new CtorDictionary()
        {
            {nameof(fileName), fileName},
            {nameof(connectionString),connectionString },
            { "nameTable", "FileContents" },
            {"nameColumn",  "FileContents"},
            {"separators",new string[]{"\r\nGO\r\n","\nGO\n","\r\nGO \r\n","\nGO \n" } },
            {"receiverProperty","sql" },
            {"columnNameWithData","FileContents" },
            {"connectionType",typeof(SqlConnection).AssemblyQualifiedName }

        })
        {
            
        }
    }
}
