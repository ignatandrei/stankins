using StankinsInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformerRelationalToPlain : ITransform
    {
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        
        List<IRow> ret ;
        void Add(IRowReceiveRelation rel)
        {
            if (rel == null)
                return;

            ret.Add(rel);
            foreach(var relationChild in rel.Relations)
            {
                foreach(var item in relationChild.Value)
                {
                    Add(item);
                }
            }
        }
        public async Task Run()
        {
            ret = new List<IRow>();
            foreach (var val in valuesRead)
            {
                
                var rel = val as IRowReceiveRelation;
                if (rel == null)
                {
                    ret.Add(val);
                    continue;
                }
                Add(rel);
                
            }
            valuesTransformed = ret.ToArray() ;
        }
    }
}
