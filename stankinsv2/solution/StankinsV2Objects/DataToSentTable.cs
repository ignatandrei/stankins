using StankinsV2Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StankinsV2Objects
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

        

        public DataTable FindAfterName(string nameTable)
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
            return t.Value;
        }
    }
}
