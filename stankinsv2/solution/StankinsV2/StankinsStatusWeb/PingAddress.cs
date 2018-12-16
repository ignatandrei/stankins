using Stankins.Alive;
using StankinsObjects;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsStatusWeb
{
    public class PingAddress : CRONExecution
    {
        
        public string NameSite { get; set; }
        private ReceiverPing cache;
        public override BaseObject baseObject()
        {
            if (cache == null)
                cache = new ReceiverPing(NameSite)
                {
                    Name = CustomData.Name
                };

            return cache;
        }

    }
}
