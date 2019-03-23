using System.Data.SqlClient;
using Stankins.FileOps;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.SqlServer
{
    public class ReceiveQueryFromFileSql : BaseObjectInSerial<ReceiverReadFileText, TransformSplitColumnAddRow, TransformerOneTableToMulti<ReceiveQueryFromDatabaseSql>>, IReceive
    {
        private readonly string fileName;
        private readonly string connectionString;

        public ReceiveQueryFromFileSql(CtorDictionary dataNeeded) : base(dataNeeded
            .AddMyValue("nameTable", "FileContents")
            .AddMyValue("nameColumn", "FileContents")
            .AddMyValue("separators", new string[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO \r\n", "\nGO \n" })
            .AddMyValue("receiverProperty", "sql")
            .AddMyValue("columnNameWithData", "FileContents")
            .AddMyValue("connectionType", typeof(SqlConnection).AssemblyQualifiedName)
            )
        {
            Name = nameof(ReceiveQueryFromFileSql);
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));
            this.connectionString = GetMyDataOrThrow<string>(nameof(connectionString));
        }

        public ReceiveQueryFromFileSql(string fileName, string connectionString) : this(new CtorDictionary()
        {
            {nameof(fileName), fileName},
            {nameof(connectionString),connectionString },

        })
        {
            
        }
    }
}
