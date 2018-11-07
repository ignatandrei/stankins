using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StankinsObjects 
{
    public class DataToSentTable : IDataToSent
    {
        public DataToSentTable()
        {
            Metadata = new MetadataTable();
            DataToBeSentFurther = new Dictionary<int, DataTable>();
        }
        public Dictionary<int,DataTable> DataToBeSentFurther { get ; set ; }
        public IMetadata Metadata { get ; set ; }

        public int AddNewTable(DataTable dt)
        {
            int max = 0;
            if(DataToBeSentFurther.Count>0 )
                max = DataToBeSentFurther.Select(it => it.Key).Max()+1;
            DataToBeSentFurther.Add(max, dt);
            return max;
            
        }

        public KeyValuePair<int, DataTable> FindAfterName(string nameTable)
        {
            var t = this.DataToBeSentFurther.FirstOrDefault(it => it.Value.TableName == nameTable);
            if (t.Value == null)
            {
                t = this.DataToBeSentFurther.FirstOrDefault(it => string.Equals(it.Value.TableName, nameTable, StringComparison.CurrentCultureIgnoreCase));

            }
            if (t.Value == null)
            {
                throw new ArgumentException($"cannot find {nameTable}");
            }
            return t;
        }
    }
}
