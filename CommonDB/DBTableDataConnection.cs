using StankinsInterfaces;
using System;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;

namespace CommonDB
{
    public class DBTableDataConnection<Connection> : IDisposable        
        where Connection : DbConnection, new()

    {
        public ISerializeData data { get; set; }
        public DBTableDataConnection(ISerializeData data)
        {
            this.data = data;
            this.Fields = new string[1] { "*" };
        }
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string[] Fields { get; set; }
        public virtual IDictionary Keys()
        {
            var c = new DbConnectionStringBuilder();
            c.ConnectionString = ConnectionString;

            return c;
        }
        private DbConnection cn;
        public virtual async Task<DbConnection> GetConnection()
        {
            //you do not multithread connections
            if (cn == null)
            {
                cn = new Connection();
                cn.ConnectionString = ConnectionString;
                await cn.OpenAsync();
            }
            return cn;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }
                if (cn != null)
                {
                    try
                    {
                        cn.Close();
                    }
                    catch
                    {
                        //do nothing...
                    }
                }
                cn = null;

                disposedValue = true;
            }
        }


        ~DBTableDataConnection()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
