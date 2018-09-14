using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transformers
{
    public class FilterRetainRelationalName : FilterRelationalName
    {

        public FilterRetainRelationalName(FilterType filter, string nameRelation) : base(filter,nameRelation)
        {
            this.Name = $"retain items that have relation matching {filter} {nameRelation}";
        }
        public override bool Result(bool exists)
        {
            return exists;
        }
    }
    public class FilterRemoveRelationalName : FilterRelationalName
    {

        public FilterRemoveRelationalName(FilterType filter, string nameRelation) : base(filter, nameRelation)
        {
            this.Name = $"remove items that have at least one key matching {filter} {nameRelation}";
        }
        public override bool Result(bool exists)
        {
            return !exists;
        }
    }
    public abstract class FilterRelationalName : IFilter
    {
        public FilterRelationalName(FilterType filter, string nameRelation)
        {

            Filter = filter;
            NameRelation = nameRelation;
        }
        private static Func<string, string, bool> search(FilterType ft)
        {
            switch (ft)
            {
                case FilterType.None:
                    return (s, k) => true;

                case FilterType.Contains:
                    return (s, k) => s.EndsWith(k);

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

        public FilterType Filter { get; set; }
        public string NameRelation { get; set; }
        public IRow[] valuesRead { get; set; }
        public IRow[] valuesTransformed { get; set; }
        public string Name { get; set; }

        List<IRowReceiveRelation> ret;
        public async Task Run()
        {
            ret=new List<IRowReceiveRelation>();
            var length = valuesTransformed?.Length;
            if (length < 1)
                return;
            if (Filter == FilterType.None)
                return;
            foreach(var item in valuesRead)
            {
                FilterRow(item as IRowReceiveRelation);
            }
            valuesTransformed = ret.ToArray();
            await Task.CompletedTask;
        }

        private void FilterRow(IRowReceiveRelation rowReceiveRelation)
        {
            if (rowReceiveRelation == null)
                return;
            foreach(var item in rowReceiveRelation.Relations)
            {
                foreach(var rr in item.Value)
                {
                    FilterRow(rr);
                }
                if (Result(search(Filter)(item.Key, NameRelation)))
                {
                    ret.AddRange(item.Value);
                }
            }
        }

        public abstract bool Result(bool exists);
    }
}

