using System;
using System.Collections.Generic;
using System.Text;
using StankinsInterfaces;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.IO;

namespace Transformers
{
    public class TransformXPath : ITransform
    {
        public TransformXPath(string keyExpressionPairs)
        {

            this.KeyExpressionPairs = keyExpressionPairs;
            this.Name = $"applying XPath expression(s) {keyExpressionPairs}";
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string KeyExpressionPairs { get; set; } //KeyTargetColumn=KeySourceColumn(event/data[@name = "wait_type"]/text/text())[1];Key02=Expr02;Key03=Expr03
        protected Dictionary<string, XPathExpression> expressionsToBeEvaluated;

        protected void Init()
        {
            expressionsToBeEvaluated = new Dictionary<string, XPathExpression>();
            string[] tokens = this.KeyExpressionPairs.Split(';');
            foreach(var token in tokens)
            {
                string[] keyExpressionPair = token.Split("=(".ToCharArray(), 3);
                expressionsToBeEvaluated.Add(keyExpressionPair[0], new XPathExpression() { SourceColumn = keyExpressionPair[1], Expression = '(' + keyExpressionPair[2] });
            }
        }

        public async Task Run()
        {
            Init();

            foreach (var row in valuesRead)
            {
                foreach(var exp in expressionsToBeEvaluated)
                {
                    if (row.Values.ContainsKey(exp.Value.SourceColumn))
                    {
                        string sourceXmlValue = (string)row.Values[exp.Value.SourceColumn];
                        if(!string.IsNullOrWhiteSpace(sourceXmlValue))
                        {
                            using (TextReader tr = new StringReader(sourceXmlValue))
                            {
                                XPathDocument document = new XPathDocument(tr);
                                XPathNavigator nav = document.CreateNavigator();
                                try
                                {
                                    row.Values[exp.Key] = nav.SelectSingleNode(exp.Value.Expression).Value;
                                }
                                catch (Exception)
                                {

                                    row.Values[exp.Key] = null;
                                }
                            }
                        }
                        else
                        {
                            row.Values[exp.Key] = null;
                        }
                    }
                    else
                    {
                        row.Values[exp.Key] = null;
                    }
                }
            }

            valuesTransformed = valuesRead;
        }

        protected struct XPathExpression
        {
            public string SourceColumn;
            public string Expression;
        }
    }
}
