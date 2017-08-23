using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    /// <summary>
    /// TODO : add test
    /// </summary>
    public class TransformRowMergeFields : TransformRow
    {
        public TransformRowMergeFields(string nameField1 , string NameField2,string nameFieldOutput, string separator="")
            : base($"Values.Add(\"{nameFieldOutput}\",Values[\"{nameField1}\"]?.ToString() + \"{separator}\"+Values[\"{NameField2}\"]?.ToString());" +
                "Values")
        {

        }
    }
}
