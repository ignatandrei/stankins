using StankinsInterfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MediaTransform
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/DOT_(graph_description_language)
    /// </summary>
    public class MediaTransformDotRelational : MediaTransformDot
    {
        public MediaTransformDotRelational():base()
        {

        }
        public MediaTransformDotRelational(string labelField):base()
        {
            LabelField = labelField;
        }

        public override async Task Run()
        {
            if (valuesToBeSent?.Length == 0)
            {
                
                return;
            }
            if (valuesToBeSent.Length != 1)
            {
                throw new ArgumentException($"values shoud have length 1, not {valuesToBeSent.Length}");
            }
            var rr = valuesToBeSent[0] as IRowReceiveRelation;
            if (rr == null)
            {
                throw new ArgumentException($"first value shoud be IRowReceiveRelation , not {valuesToBeSent[0]?.GetType()}");
            }
            
            var sb = new StringBuilder();
            sb.AppendLine($"digraph {LabelField}");
            sb.AppendLine("{");

            
            
            sb.AppendLine(AppendDataForParent(rr, null,LabelField));
            

            sb.AppendLine("}");
            Result = sb.ToString();
            await Task.CompletedTask;
            
        }

        private string AppendDataForParent(IRowReceiveRelation rr, string parentRelNode, string label)
        {
            var sb = new StringBuilder();
            var otherAttributes = OtherAttributes(rr);
            sb.AppendLine($"Node{rr.ID} [label=\"{rr.Values[label]}\" {otherAttributes}];");
            if(parentRelNode != null)
            {
                sb.AppendLine($"{parentRelNode} -> Node{rr.ID};");
            }
            var rels = rr.Relations;
            foreach(var relName in rels.Keys)
            {
                if (rels[relName].Count == 0)
                    continue;
                string relNode = $"NodeRel{relName + "_" + rr.ID}";
                sb.AppendLine($"{relNode} [label=\"{relName}\"];");
                sb.AppendLine($"Node{rr.ID} -> {relNode};");
                foreach(var childRel in rels[relName])
                {
                    sb.AppendLine(AppendDataForParent(childRel, relNode, label));
                }
            }

            return sb.ToString();
        }
    }
}
