using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterRemainItemsForValue: FilterAddRemoveItemsForValue
    {
        public FilterRemainItemsForValue(string key, string valueSearch, FilterType filterType)
            : base(key, valueSearch, filterType)
        {

        }
        public override bool Result(bool exists)
        {
            return exists;
        }
    }
    public class FilterRemoveItemsForValue : FilterAddRemoveItemsForValue
    {
        public FilterRemoveItemsForValue(string key, string valueSearch, FilterType filterType)
            : base(key, valueSearch, filterType)
        {

        }
        public override bool Result(bool exists)
        {
            return !exists;
        }
    }
    public abstract class FilterAddRemoveItemsForValue : IFilter
    {
        public FilterAddRemoveItemsForValue(string key,string valueSearch, FilterType filterType)
        {
            Key = key;
            ValueSearch = valueSearch;
            FilterType = filterType;

        }

        public string Key { get; }
        public string ValueSearch { get; }

        public FilterType FilterType { get; set; }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }
        private static Func<string, string, bool> search(FilterType ft)
        {
            switch (ft)
            {
                case FilterType.None:
                    return (s, k) => true;

                case FilterType.Contains:
                    return (s, k) => s.Contains(k);

                case FilterType.StartsWith:
                    return (s, k) => s.StartsWith(k);
                case FilterType.Endswith:
                    return (s, k) => s.EndsWith(k);
                case FilterType.Equal:
                    return (s, k) => s == k;

                default:
                    throw new ArgumentException($"not found {ft}");

            }

        }
        public async Task Run()
        {
            valuesTransformed = valuesRead;
            var length = valuesTransformed?.Length;
            if (length < 1)
                return;
            if (FilterType == FilterType.None)
                return;
            var ret = new List<IRow>();
            foreach (var item in valuesRead)
            {
                if (!(item.Values?.ContainsKey(Key)??false))
                    continue;

                var val = item.Values[Key];
                
                if (string.IsNullOrWhiteSpace(val?.ToString()))
                    continue;

                var contains = search(FilterType)(val?.ToString(), ValueSearch);

                if (Result(contains))
                    ret.Add(item);

            }
            valuesTransformed = ret.ToArray();
            await Task.CompletedTask;
        }
        public abstract bool Result(bool exists);
    }
}
