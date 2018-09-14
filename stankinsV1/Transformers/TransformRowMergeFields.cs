using System.Linq;
using System.Text;

namespace Transformers
{
    /// <summary>
    /// TODO : add test
    /// </summary>
    public class TransformRowMergeFields : TransformRow
    {
        
        public TransformRowMergeFields(string nameField1 , string nameField2,string nameFieldOutput, string separator="")
            : base($"Values.Add(\"{nameFieldOutput}\",Values[\"{nameField1}\"]?.ToString() + \"{separator}\"+Values[\"{nameField2}\"]?.ToString());" +
                "Values")
        {
            NameField1 = nameField1;
            NameField2 = nameField2;
            NameFieldOutput = nameFieldOutput;
            Separator = separator;
        }
        protected override string ModifyExpression(string theExpression)
        {
            return $"Values.Add(\"{NameFieldOutput}\",Values[\"{NameField1}\"]?.ToString() + \"{Separator}\"+Values[\"{NameField2}\"]?.ToString());" +
                "Values";
        }
        public string NameField1 { get; set; }
        public string NameField2 { get; set; }
        public string NameFieldOutput { get; set; }
        public string Separator { get; set; }
    }
}
