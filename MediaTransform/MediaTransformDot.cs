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
    public class MediaTransformDot : MediaTransformString
    {
        public MediaTransformDot(string labelField):base()
        {
            this.LabelField = labelField;
        }
        public string LabelField { get; set; }
        string AppendDataForParent(IRowReceiveHierarchical[] col, IRowReceiveHierarchical parent, string label)
        {
            if (parent == null)
                return null;
            var sb = new StringBuilder();
            if (parent.Parent == null)
            {
                sb.AppendLine($"Node{parent.ID} [label=\"{parent.Values[label]}\"];");
            }

            var children = col.Where(it => it.Parent == parent).ToArray();
            if (children?.Length == 0)
                return "";

            
            foreach (var item in children)
            {
                sb.AppendLine($"Node{item.ID} [label=\"{item.Values[label]}\"];");
                sb.AppendLine($"Node{parent.ID} -> Node{item.ID};");
                sb.AppendLine(AppendDataForParent(col, item, label));
            }
            return sb.ToString();
        }
        public override async Task Run()
        {
            var data=valuesToBeSent.Select(it => it as IRowReceiveHierarchical).Where(it=>it!=null).ToArray();
            var sb = new StringBuilder();
            sb.AppendLine($"digraph {LabelField}");
            sb.AppendLine("{");

            foreach (var item in data.Where(it => it.Parent == null))
            {
                sb.AppendLine(AppendDataForParent(data,item,LabelField));
            }

            sb.AppendLine("}");
            Result = sb.ToString();

        }
    }
}
