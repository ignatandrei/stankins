using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Stankins.XML
{
    public class TransformerXMLToColumn : BaseObject, ITransformer
    {
        public TransformerXMLToColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            this.ColumnName = base.GetMyDataOrThrow<string>(nameof(ColumnName));
            this.XPath = base.GetMyDataOrThrow<string>(nameof(XPath));
            this.NewColumnName = base.GetMyDataOrThrow<string>(nameof(NewColumnName));
            this.Separator = base.GetMyDataOrDefault<string>(nameof(Separator),","); 
        }
        public TransformerXMLToColumn(string columnName, string xPath,string newColumnName, string separator)  :this(new CtorDictionary()
        {
            { nameof(columnName),columnName },
                { nameof(xPath),xPath },
                { nameof(newColumnName),newColumnName },
            { nameof(separator),separator }
        })
        {
            
        }
        public string ColumnName { get; }
        public string XPath { get; }
        public string NewColumnName { get; }
        public string Separator { get; }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tableAndPosition = FindTableAfterColumnName(ColumnName, receiveData);
            foreach (var tablePosition in tableAndPosition)
            {
                var table = tablePosition.Value;
                table.Columns.Add(NewColumnName);
                receiveData.Metadata.Columns.Add(new Column() { IDTable = tablePosition.Key, Name = NewColumnName, Id = receiveData.Metadata.Columns.Count });
                foreach (DataRow dr in table.Rows)
                {
                    var val = dr[ColumnName]?.ToString();
                    dr[NewColumnName] = val;
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }
                    using (var sr = new StringReader(val)) { 
                        var document = new XPathDocument(sr);
                        var navig = document.CreateNavigator();
                        var nodes = navig.Select(XPath);
                        var newVal = new List<String>();
                        while (nodes.MoveNext())
                        {
                            var n = nodes.Current;
                            newVal.Add(n.Value);
                        }
                        dr[NewColumnName] = string.Join(Separator,newVal);
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
