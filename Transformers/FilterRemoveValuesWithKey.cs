﻿using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Transformers
{
    public enum FilterType
    {
        None=0,
        Contains=1,
        StartsWith=2,
        Endswith=3,
        Equal=4

    }
    public class FilterRetainValuesWithKey : FilterAddRemoveValuesWithKey
    {

        public FilterRetainValuesWithKey(string key, FilterType filterType) : base(key, filterType)
        {

        }
        public override bool Result(bool exists)
        {
            return exists;
        }
    }
    public  class FilterRemoveValuesWithKey : FilterAddRemoveValuesWithKey
    {

        public FilterRemoveValuesWithKey(string key, FilterType filterType):base(key,filterType)
        {
        
        }
        public override bool Result(bool exists)
        {
            return !exists;
        }
    }
    public abstract class FilterAddRemoveValuesWithKey: IFilter
    {
        public FilterAddRemoveValuesWithKey(string key, FilterType filterType)
        {
            Key = key;
            FilterType = filterType;

        }

        public string Key { get; set; }
        public FilterType FilterType { get; set; }
        public IRow[] valuesRead { get ; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
        public async Task Run()
        {
            valuesTransformed = valuesRead;
            var length = valuesTransformed?.Length;
            if (length < 1)
                return;
            if (FilterType == FilterType.None)
                return;
            var ret= new List<IRow>();
            foreach (var item in valuesRead)
            {
                var keys = item.Values.Keys.ToArray();
                var contains = keys.Any(it => search(FilterType)(it, Key));

                if (Result(contains))
                    ret.Add(item);
                
            }
            valuesTransformed = ret.ToArray();
            await Task.CompletedTask;
        }
        public abstract bool Result(bool exists);
    }
}