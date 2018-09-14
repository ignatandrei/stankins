using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Transformers
{
    public class TransformAddFieldFormatter : TransformRow
    {
        public TransformAddFieldFormatter(string nameFieldOutput, string stringFormatExpression):base("")
        {
            NameFieldOutput = nameFieldOutput;
            StringFormatExpression = stringFormatExpression;
        }

        public string NameFieldOutput { get; set; }
        public string StringFormatExpression { get; set; }
        protected override string ModifyExpression(string theExpression)
        {
            string s=FormatWith(StringFormatExpression);
            s = $"Values.Add(\"{NameFieldOutput}\"," +"\""+ s + "\"" + "); Values";
            return s;
        }
        //copied after https://haacked.com/archive/2009/01/14/named-formats-redux.aspx/
        public static string FormatWith(string format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            List<object> values = new List<object>();
            string rewrittenFormat = Regex.Replace(format,
              @"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
              delegate (Match m)
              {
                  Group startGroup = m.Groups["start"];
                  Group propertyGroup = m.Groups["property"];
                  Group formatGroup = m.Groups["format"];
                  Group endGroup = m.Groups["end"];

                  //      values.Add((propertyGroup.Value == "0")
                  //? source
                  //: Eval(source, propertyGroup.Value));
                  values.Add(Eval(propertyGroup.Value));
                  int openings = startGroup.Captures.Count;
                  int closings = endGroup.Captures.Count;

                  return openings > closings || openings % 2 == 0
             ? m.Value
             : new string('{', openings) + (values.Count - 1)
               + formatGroup.Value
               + new string('}', closings);
              },
              RegexOptions.Compiled
              | RegexOptions.CultureInvariant
              | RegexOptions.IgnoreCase);

            return string.Format(rewrittenFormat,values.ToArray());
        }

        private static string Eval(string expression)
        {
            return "\"+"+ $"Values[\"{expression}\"]?.ToString()" + "+\"";
            
        }
    }
}
