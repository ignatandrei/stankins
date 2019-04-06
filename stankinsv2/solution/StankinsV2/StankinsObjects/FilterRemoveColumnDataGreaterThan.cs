using StankinsCommon;

namespace StankinsObjects
{
    public class FilterRemoveColumnDataGreaterThan: FilterColumnData
    {
        private readonly string nameColumn;
        private readonly int value;

        public FilterRemoveColumnDataGreaterThan(string nameColumn, int value) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(value), value}
            }
           )
        {
            
            this.nameColumn = nameColumn;
            this.value = value;
        }
       
        public FilterRemoveColumnDataGreaterThan(CtorDictionary dataNeeded) : base(
            new CtorDictionary(dataNeeded) {
            { nameof(Expression), $"{dataNeeded[nameof(nameColumn)]}>{dataNeeded[nameof(value)]}"}
            }
           )
            
        {
            this.Name = nameof(FilterRemoveColumnDataGreaterThan);
        }

    }
}