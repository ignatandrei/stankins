using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MediaTransform
{
    public class MediaTransformXML : MediaTransformString
    {
        public string RootName { get; set; }

        public MediaTransformXML(string rootName)
        {
            this.RootName = rootName;
        }
        public override async Task Run()
        {
            if (valuesToBeSent?.Length == 0)
            {
                //LOG: there are no data 
                return;
            }
            var sb = new StringBuilder();
            var FieldNameToMarks = valuesToBeSent[0]
                .Values
                .Select(it => it.Key).ToArray();

            //sb.AppendLine(string.Join(",", FieldNameToMarks));
            var nrValues = valuesToBeSent.LongCount();
            var settings = new XmlWriterSettings();
            settings.Async = false;


            using (var writer = XmlWriter.Create(sb, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RootName ?? "values");
                for (var i = 0; i < nrValues; i++)
                {
                    var row = valuesToBeSent[i];
                    foreach (var fieldName in FieldNameToMarks)
                    {
                        var val = row.Values.FirstOrDefault(it => it.Key == fieldName);

                        var str = val.Value?.ToString();

                        writer.WriteStartElement(fieldName);
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            writer.WriteString(str);
                        }
                        writer.WriteEndElement();


                    }
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
            Result = sb.ToString();
        }
    }
}
