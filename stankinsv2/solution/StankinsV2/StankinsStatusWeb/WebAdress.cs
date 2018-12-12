using Stankins.Alive;
using StankinsObjects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class WebAdress: CRONExecution, IToBaseObject
    {
        public string URL { get; set; }
        public CustomData CustomData { get; set; }

        private ReceiverWeb cache;
        public BaseObject baseObject()
        {
            if(cache == null)
                cache=new ReceiverWeb(URL)
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
