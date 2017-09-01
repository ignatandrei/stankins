using StankinsInterfaces;
using System.Threading.Tasks;

namespace MediaTransform
{
    public abstract class MediaTransformByte: IFilterTransformToByteArray
    {
        public IRow[] valuesToBeSent { set; protected get; }

        public byte[] Result { get; protected set; }

        public abstract  Task Run();
        public string Name { get; set; }
    }
}
