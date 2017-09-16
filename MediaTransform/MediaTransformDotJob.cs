using StankinsInterfaces;

namespace MediaTransform
{
    public class MediaTransformDotJob: MediaTransformDotHierarchical
    {
        public MediaTransformDotJob():base("Name")
        {

        }
        public override string OtherAttributes(IRowReceive row)
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
}
