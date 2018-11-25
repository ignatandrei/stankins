using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Alive
{
    public abstract class AliveStatus : BaseObject, IReceive
    {
        public AliveStatus(CtorDictionary dataNeeded) : base(dataNeeded)
        {

        }
        public override async Task<IMetadata> TryLoadMetadata()
        {
            var m = new MetadataTable();
            m.Tables.Add(new Table()
            {
                Name = this.Name
            });
            var idTable = m.Tables.First(it => it.Name == this.Name).Id;
            m.AddColumn("Process", idTable);
            m.AddColumn("Arguments", idTable);
            m.AddColumn("To", idTable);
            m.AddColumn("Result", idTable);
            m.AddColumn("DetailedResult", idTable);
            return m;
        }
    }
}
