using StankinsInterfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Transformers
{
    public class TransformRowRemainsProperties : ITransform
    {
        public TransformRowRemainsProperties(params string[] props)
        {

            Properties = props;
        }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        public string[] Properties { get; set; }

        public async Task Run()
        {
            bool mustTransform = Properties?.Length > 0;
            if (mustTransform)
            {
                foreach (var item in valuesRead)
                {

                    var keys = item.Values.Keys.ToArray();
                    var diff = keys.Except(Properties).ToArray();
                    if (diff?.Length < 1)
                        continue;
                    foreach (var key in diff)
                    {
                        item.Values.Remove(key);
                    }


                }
            }
            valuesTransformed = valuesRead;
        }
    }
}
