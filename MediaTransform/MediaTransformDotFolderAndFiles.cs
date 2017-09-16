using StankinsInterfaces;

namespace MediaTransform
{
    public class MediaTransformDotFolderAndFiles : MediaTransformDotHierarchical
    {
        public MediaTransformDotFolderAndFiles():base("Name")
        {

        }
        public override string OtherAttributes(IRowReceive parent)
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
}
