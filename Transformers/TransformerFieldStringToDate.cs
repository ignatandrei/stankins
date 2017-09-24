using System;

namespace Transformers
{
    public class TransformerFieldStringToDate : TransformOneValue<string,DateTime>
    {
        
        public TransformerFieldStringToDate
            (string oldField, string newField, string format="") :
            base("", oldField, newField)
        {
            Format = format;
        }

        public string Format { get; }
        string expression;
        public override string TheExpression {
            get {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    if (string.IsNullOrWhiteSpace(Format))
                    {
                        expression="System.DateTime.Parse(oldValue.ToString())";
                    }
                    else
                    {
                        expression = $"System.DateTime.ParseExact(oldValue.ToString(),\"{Format}\",null)";
                    }
                }
                return expression;
            }
            set {
                expression = value;
            }
        }
    }
}
