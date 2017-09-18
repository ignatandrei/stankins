using CommonDB;
using StankinsInterfaces;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SenderToDB
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SenderSqlToDB<T> : ISend,IDisposable   
        where T:DbConnection, new()
    {
        public SenderSqlToDB(DBDataConnection<T> connection)
        {
            OrderBy = "FullName";
            Connection = connection;
        }
        public IRow[] valuesToBeSent { get; set; }
        public string Name { get ; set ; }
        public string OrderBy { get; set; }
        public DBDataConnection<T> Connection { get; set; }
        public virtual string[] SplitSql(string fullText)
        {
            return new string[]{ fullText };
        }
        public async Task Send()
        {
            var cn = await Connection.GetConnection();
            var t= cn.BeginTransaction(); 
            try
            {
                
                var order = valuesToBeSent.OrderBy(it => it.Values[OrderBy]);

                foreach (var item in order)
                {
                    if (!item.Values.ContainsKey("FullName"))
                    {
                        //TODO: log
                        continue;
                    }
                    string fullName = item.Values["FullName"].ToString();

                    if (!File.Exists(fullName))
                        continue;

                    var sql = File.ReadAllText(fullName);
                    foreach (var sqlStatement in SplitSql(sql))
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = t;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sqlStatement;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                t.Commit();
            }
            catch(Exception ex)
            {
                string s = ex.Message;
                if (t != null)
                    t.Rollback();
                
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                if(Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
                valuesToBeSent = null;
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SenderSqlToDB()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
