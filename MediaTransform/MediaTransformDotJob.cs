using StankinsInterfaces;
using Microsoft.Extensions.Logging;
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
                string message = $"no key for RowType";
                //@class.Log(LogLevel.Information, 0, $"media transform dot job : {message}", null, null);                        
                message += "";
                return null;
            }
            var str = row.Values["RowType"]?.ToString();
            

            switch (str.ToLowerInvariant())
            {
                case "sender":
                    return "shape=signature color=lime";
                case "filter_transformer":
                case "filter":
                case "transformer":
                    return "shape=invhouse   color=lightgrey";

                case "receiver":
                    return "shape=cylinder color=lightblue";
                default:
                    string message = $"no shape defined for {str}";
                    //@class.Log(LogLevel.Information, 0, $"media transform dot job: {message}", null, null);                        
                    message += "";
                    return "";
            }

        }
    }
}
