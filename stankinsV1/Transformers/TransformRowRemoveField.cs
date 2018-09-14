using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    /// <summary>
    /// TODO: make this easy by removing data from dictionary
    /// </summary>
    public class TransformRowRemoveField: TransformRow
    {
        public TransformRowRemoveField(params string[] nameFields)
            : base("")
        {
            NameFields = nameFields;
        }

        public string[] NameFields { get; }

        protected override string ModifyExpression(string theExpression)
        {
            var sb = new StringBuilder();
            foreach(var NameField in NameFields)
            {
                sb.Append($"Values.Remove(\"{NameField}\");");
            }
            return sb.ToString()+"Values";
        }
    }
}
