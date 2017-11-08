using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Transformers;

namespace BlocklyClasses
{
    public class FilterColumnValue : IFilter
    {
        public FilterColumnValue(string columnName, string valueSearch, FilterType filterType, bool invertCondition)            
        {
            ColumnName = columnName;
            ValueSearch = valueSearch;
            FilterType = filterType;
            InvertCondition = invertCondition;
        }
        public IRow[] valuesRead { get ; set ; }
        public IRow[] valuesTransformed { get ; set ; }
        public string Name { get ; set ; }
        public string ColumnName { get; set; }
        public string ValueSearch { get; set; }
        public FilterType FilterType { get; set; }
        public bool InvertCondition { get; set; }

        public async Task Run()
        {
            IFilter filter;
            if (InvertCondition)
            {
                filter = new FilterRemoveItemsForValue(ColumnName, ValueSearch, FilterType);
            }
            else
            {
                filter = new FilterRemainItemsForValue(ColumnName, ValueSearch, FilterType);
            }
            filter.valuesRead = this.valuesRead;
            await filter.Run();
            valuesTransformed = filter.valuesTransformed;

        }
    }
}
