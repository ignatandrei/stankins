using StankinsInterfaces;

namespace MediaTransform
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/DOT_(graph_description_language)
    /// </summary>
    public abstract class MediaTransformDot: MediaTransformString
    {
        public string LabelField { get; set; }
        public virtual string OtherAttributes(IRowReceive parent)
        {
            return null;
        }
    }
}
