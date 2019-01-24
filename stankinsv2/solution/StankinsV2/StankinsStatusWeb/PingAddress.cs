using Stankins.Alive;
using StankinsObjects;
using System.Collections.Generic;
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
                    Name = string.IsNullOrWhiteSpace(CustomData.Name) ? NameSite : CustomData.Name
                };

            return cache;
        }

    }

    public class PingAddresses : CRONExecutionMultiple<PingAddress>
    {

        public string NameSite { get; set; }
        public string CRON { get; set; }
        public IEnumerable<PingAddress> Multiple()
        {
            foreach(var item in NameSite.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
            {
                yield return new PingAddress()
                {
                    NameSite = item.Trim(),
                    CRON = this.CRON
                };
            }
        }


    }
}
