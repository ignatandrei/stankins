using System;
using System.Data;
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
        protected DataTable CreateTable()
        {
            var m = new DataTable();
            m.Columns.Add("Process",typeof(string));
            m.Columns.Add("Arguments", typeof(string));
            m.Columns.Add("To", typeof(string));
            m.Columns.Add("Result", typeof(string));
            m.Columns.Add("DetailedResult", typeof(string));
            m.Columns.Add("Exception", typeof(string));
            return m;
        }

        public override async Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();

        }
    }
}
