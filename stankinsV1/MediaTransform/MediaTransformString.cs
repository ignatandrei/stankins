using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediaTransform
{
    public class MediaTransformMaxMin<T> : IFilterTransformTo<T>
         where T : IComparable<T>
    {
        public MediaTransformMaxMin()
        {

        }
        public IRow[] valuesToBeSent { set; get; }

        public T Result { get; set; }
        public GroupingFunctions GroupFunction { get; set; }
        public string Name { get ; set ; }
        public string FieldName { get; set; }

        public async Task Run()
        {
            var type = typeof(T);
            var vals = valuesToBeSent.Select(it =>
                it.Values.ContainsKey(FieldName) ?
                it.Values[FieldName] : null)
                .Where(it => it != null)
                .Select(it => (T)Convert.ChangeType(it, type))
                .ToArray();
            
            switch (GroupFunction)
            {
                case GroupingFunctions.Min:
                    Result = vals.Min();
                    break;
                case GroupingFunctions.Max:
                    Result = vals.Max();
                    break;
                default:
                    throw new ArgumentException($"cannot find {GroupFunction}");
            }
        }
    }
    public abstract class MediaTransformString : IFilterTransformToString
    {
        public IRow[] valuesToBeSent { set; protected get; }

        public string Result { get; protected set; }

        public abstract  Task Run();
        public string Name { get; set; }
    }
}
