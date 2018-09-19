using HtmlAgilityPack;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsV2Objects;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Stankins.HTML
{
    public class TransformerHTMLAttribute : BaseObject, ITransformer
    {
        public TransformerHTMLAttribute(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.ColumnName = base.GetMyDataOrThrow<string>(nameof(ColumnName));
            this.AttributeName = base.GetMyDataOrThrow<string>(nameof(AttributeName));
            this.NewColumnName= base.GetMyDataOrThrow<string>(nameof(NewColumnName));

        }
        public TransformerHTMLAttribute(string columnName, string attribute) :
            this(columnName, attribute, columnName + attribute)
        {

        }
        public TransformerHTMLAttribute(string columnName, string attributeName, string newColumnName) : this(new CtorDictionary()
            {
                {nameof(columnName),columnName },
                {nameof(attributeName),attributeName },
                {nameof(newColumnName),newColumnName }
            })
        {
            
        }

        public string ColumnName { get; }
        public string AttributeName { get; }
        public string NewColumnName { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tableAndPosition = FindTableAfterColumnName(ColumnName, receiveData);                
            foreach (var tablePosition in tableAndPosition)
            {
                var table = tablePosition.Value;
                table.Columns.Add(NewColumnName);
                receiveData.Metadata.Columns.Add(new Column() { IDTable = tablePosition.Key, Name = NewColumnName, Id = receiveData.Metadata.Columns.Count });
                foreach(DataRow dr in table.Rows)
                {
                    var val = dr[ColumnName]?.ToString();
                    dr[NewColumnName] = val;
                    if (string.IsNullOrWhiteSpace(val))
                    {                        
                        continue;
                    }
                    var doc = new HtmlDocument();
                    doc.LoadHtml(val);
                    var attr = doc.DocumentNode.SelectNodes("//@" + AttributeName).FirstOrDefault();
                    if(attr != null)
                    {
                        dr[NewColumnName] = attr.Attributes[AttributeName].Value;
                        continue;
                    }

                }
            }
            return receiveData;

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
