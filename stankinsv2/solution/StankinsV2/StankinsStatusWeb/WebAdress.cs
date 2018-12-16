using Stankins.Alive;
using StankinsObjects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class WebAdress: CRONExecution, IToBaseObjectExecutable
    {
        public string URL { get; set; }
       
        private ReceiverWeb cache;
        public override  BaseObject baseObject()
        {
            if(cache == null)
                cache=new ReceiverWeb(URL)
            {
                Name = CustomData.Name
            };

            return cache;
        }

       
        
    }
}
