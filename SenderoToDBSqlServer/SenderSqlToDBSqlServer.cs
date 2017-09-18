using CommonDB;
using SenderToDB;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace SenderToDBSqlServer
{
    public class SenderSqlToDBSqlServer: SenderSqlToDB<SqlConnection>
    {
        public SenderSqlToDBSqlServer(DBDataConnection<SqlConnection> connection):base(connection){

        }
        public override string[] SplitSql(string fullText)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(fullText);
            lines = lines.Where(it => !string.IsNullOrWhiteSpace(it)).ToArray();
            return lines;
        }
    }
}
