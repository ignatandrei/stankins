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
    public class MediaTransformDotRelational : MediaTransformString
    {
        public override async Task Run()
        {
            await Task.CompletedTask;
        }
    }
    public class MediaTransformDotFolderAndFiles : MediaTransformDotHierarchical
    {
        public MediaTransformDotFolderAndFiles():base("Name")
        {

        }
        public override string OtherAttributes(IRowReceiveHierarchicalParent parent)
        {
            var str = parent.Values["RowType"]?.ToString();
            switch (str)
            {
                case "folder":
                    return "shape=folder color=lightblue";
                case "file":
                    return "shape=now color=lightgrey";
                default:
                    //TODO: log
                    return "";
            }
        }
    }
    public class MediaTransformDotJob: MediaTransformDotHierarchical
    {
        public MediaTransformDotJob():base("Name")
        {

        }
        public override string OtherAttributes(IRowReceiveHierarchicalParent row)
        {
            if (!(row?.Values?.ContainsKey("RowType") ?? false))
            {
                //TODO: log                
                return null;
            }
            var str = row.Values["RowType"]?.ToString();
            

            switch (str.ToLowerInvariant())
            {
                case "sender":
                    return "shape=signature color=lime";
                case "filter_transformer":
                    return "shape=invhouse   color=lightgrey";
                case "receiver":
                    return "shape=cylinder color=lightblue";
                default:
                    //TODO: log
                    return "";
            }

        }
    }
    /// <summary>
    /// https://en.wikipedia.org/wiki/DOT_(graph_description_language)
    /// </summary>
    public class MediaTransformDotHierarchical : MediaTransformString
    {
        public MediaTransformDotHierarchical(string labelField):base()
        {
            this.LabelField = labelField;
        }
        public virtual string OtherAttributes(IRowReceiveHierarchicalParent parent)
        {
            return null;
        }
        public string LabelField { get; set; }

        string AppendDataForParent(IRowReceiveHierarchicalParent[] col, IRowReceiveHierarchicalParent parent, string label)
        {
            if (parent == null)
                return null;
            var sb = new StringBuilder();
            if (parent.Parent == null)
            {
                var otherAttributes = OtherAttributes(parent);
                sb.AppendLine($"Node{parent.ID} [label=\"{parent.Values[label]}\" {otherAttributes}];");
            }

            var children = col.Where(it => it.Parent == parent).ToArray();
            if (children?.Length == 0)
                return sb.ToString();

            
            foreach (var item in children)
            {
                var otherAttributes = OtherAttributes(item);
                sb.AppendLine($"Node{item.ID} [label=\"{item.Values[label]}\" {otherAttributes}];");
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
