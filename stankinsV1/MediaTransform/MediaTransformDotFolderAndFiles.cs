using StankinsInterfaces;
using Microsoft.Extensions.Logging;
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
                    string message = $"no shape defined for {str}";
                    //@class.Log(LogLevel.Information, 0, $"media dot folder and files: {message}", null, null);                        
                    message += "";
                    return "";
            }
        }
    }
}
