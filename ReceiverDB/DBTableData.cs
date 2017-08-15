using StankinsInterfaces;
using System;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;

namespace ReceiverDB
{
    public class DBTableData<T, Connection> : IDisposable
        where T : IComparable<T>
        where Connection : DbConnection, new()

    {
        public ISerializeData data { get; set; }
        public DBTableData(ISerializeData data)
        {
            this.data = data;
            this.Fields = new string[1] { "*" };
        }
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string[] Fields { get; set; }
        //TODO: add multiple fields - lastcreated or modified
        public string FieldNameToMark { get; set; }

        public long MaxRecordsToRead = long.MaxValue;
        //public long PageNumber=1;
        public T lastValue
        {
            get
            {
                var dt =data.GetValue($"{TableName}_{FieldNameToMark}");
                if (dt == null)
                    return default(T);

                return (T)Convert.ChangeType(dt, typeof(T));
            }
            set
            {
                data.SetValue($"{TableName}_{FieldNameToMark}", value); 
                
            }
        }
        public IDictionary Keys()
        {
            var c = new DbConnectionStringBuilder();
            c.ConnectionString = ConnectionString;
            
            c.Add($"{nameof(FieldNameToMark)}", $"{FieldNameToMark}");
            c.Add($"{nameof(TableName)}", $"{TableName}");
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
                    cn.Close();
                }
                cn = null;
                
                disposedValue = true;
            }
        }

        
         ~DBTableData() {
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
