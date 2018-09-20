using Stankins.Interfaces;
using StankinsCommon;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public abstract class Receiver :BaseObject, IReceive
    {
        public Receiver(CtorDictionary dataNeeded):base(dataNeeded)
        {

        }

        

        public virtual async Task<IMetadata> LoadMetadata()
        {
            var data = await TransformData(null);
            return data.Metadata;
        }

        
    }
}
