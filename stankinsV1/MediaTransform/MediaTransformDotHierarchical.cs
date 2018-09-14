using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/DOT_(graph_description_language)
    /// </summary>
    public class MediaTransformDotHierarchical : MediaTransformDot
    {
        public MediaTransformDotHierarchical(string labelField):base()
        {
            LabelField = labelField;
        }
        
        

        string AppendDataForParent(IRowReceiveHierarchicalParent[] col, IRowReceiveHierarchicalParent parent, string label)
        {
            if (parent == null)
                return null;
            var sb = new StringBuilder();
            if (parent.Parent == null)
            {
                var otherAttributes = OtherAttributes(parent);
                sb.AppendLine($"Node{parent.ID} [label=\"{ReplaceStringForJavascript(parent.Values[label].ToString())}\" {otherAttributes}];");
            }

            var children = col.Where(it => it.Parent == parent).ToArray();
            if (children?.Length == 0)
                return sb.ToString();

            
            foreach (var item in children)
            {
                var otherAttributes = OtherAttributes(item);
                sb.AppendLine($"Node{item.ID} [label=\"{ReplaceStringForJavascript(item.Values[label].ToString())}\" {otherAttributes}];");
                sb.AppendLine($"Node{parent.ID} -> Node{item.ID};");
                sb.AppendLine(AppendDataForParent(col, item, label));
            }
            return sb.ToString();
        }
        public override async Task Run()
        {
            var data=valuesToBeSent.Select(it => it as IRowReceiveHierarchicalParent).Where(it=>it!=null).ToArray();
            var sb = new StringBuilder();
            sb.AppendLine($"digraph {LabelField}");
            sb.AppendLine("{");

            foreach (var item in data.Where(it => it.Parent == null))
            {
                sb.AppendLine(AppendDataForParent(data,item,LabelField));
            }

            sb.AppendLine("}");
            Result = sb.ToString();
            await Task.CompletedTask;
        }
    }
}
