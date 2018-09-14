using CommonDB;
using StankinsInterfaces;
using System;
using System.Collections;
using System.Data.Common;

namespace ReceiverDB
{
    public class DBTableData<T, Connection> : DBTableDataConnection<Connection>
        where T : IComparable<T>
        where Connection : DbConnection, new()

    {
        
        public DBTableData(ISerializeData data):base(data)
        {
            
            this.Fields = new string[1] { "*" };
        }
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
        public override IDictionary Keys()
        {
            var c = base.Keys();
            c.Add($"{nameof(FieldNameToMark)}", $"{FieldNameToMark}");
            c.Add($"{nameof(TableName)}", $"{TableName}");
            return c;
        }
        
    }
}
