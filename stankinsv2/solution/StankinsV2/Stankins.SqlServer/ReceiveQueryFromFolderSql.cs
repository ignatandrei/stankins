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
    public class ReceiveQueryFromFolderSql : BaseObjectInSerial<ReceiverFilesInFolder, TransformerOneTableToMulti<ReceiverReadFileText>, FilterTablesWithColumn,TransformerToOneTable, TransformerOneTableToMulti< ReceiveQueryFromDatabaseSql>>, IReceive
    {
        public ReceiveQueryFromFolderSql(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.Name = nameof(ReceiveQueryFromFolderSql);
        }

        public ReceiveQueryFromFolderSql(string pathFolder, string filter, string connectionString) : this(new CtorDictionary()
        {
            {nameof(pathFolder), pathFolder},
            {nameof(filter),filter },
            {nameof(connectionString),connectionString },
            {"NameColumnToKeep","FileContents" },
            {"receiverProperty","fileName|sql" },
            {"columnNameWithData","FullFileName|FileContents" },
            {"connectionType",typeof(SqlConnection).FullName }

        })
        {

        }
    }
}
