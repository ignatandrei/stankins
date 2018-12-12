using Stankins.Alive;
using StankinsObjects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class PingAddress : CRONExecution, IToBaseObject
    {
        public CustomData CustomData { get; set; }
        public string NameSite { get; set; }
        private ReceiverPing cache;
        public BaseObject baseObject()
        {
            if(cache == null)
            cache = new ReceiverPing(NameSite)
            {
                Name = CustomData.Name
            };

            return cache;
        }

        public async Task<DataTable> Execute()
        {
            
            var ret = await baseObject().TransformData(null);
            return ret.DataToBeSentFurther.Values.First();
        }


    }
}
