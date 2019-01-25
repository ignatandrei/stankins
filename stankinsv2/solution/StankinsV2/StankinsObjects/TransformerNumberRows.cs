using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformerAggregateString:BaseObject, ITransformer
    {
        public TransformerAggregateString(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            NameColumn= base.GetMyDataOrThrow<string>(nameof(NameColumn));
            Separator = base.GetMyDataOrDefault<string>(nameof(Separator), Environment.NewLine);
        }
        public TransformerAggregateString(string nameColumn,string separator) : this(new CtorDictionary()
        {
            {nameof(nameColumn),nameColumn},
            {nameof(separator),separator }

        })
        {
            NameColumn = nameColumn;
            Separator = separator;
        }

        public string NameColumn { get; }
        public string Separator { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = base.FindTableAfterColumnName(NameColumn, receiveData).ToArray();
            foreach(var t in tables)
            {
                var table = t.Value;
                
                var values = new List<string>();
                foreach(DataRow dr in table.Rows)
                {
                    values.Add( dr[NameColumn]?.ToString());
                    
                }
                var dt = new DataTable();
                dt.Columns.Add(dt.TableName + "_" + NameColumn);
                dt.Rows.Add(string.Join(Separator, values.ToArray()));
                var id = receiveData.AddNewTable(dt);
                receiveData.Metadata.AddTable(dt, id);
            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
