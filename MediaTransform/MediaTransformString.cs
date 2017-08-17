using StankinsInterfaces;
using System.Threading.Tasks;

namespace MediaTransform
{
    public abstract class MediaTransformString : IFilterTransformToString
    {
        public IRow[] valuesToBeSent { set; protected get; }

        public string Result { get; protected set; }

        public abstract  Task Run();
        
    }
}
