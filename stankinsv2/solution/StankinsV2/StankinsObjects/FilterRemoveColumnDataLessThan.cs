using StankinsCommon;

namespace StankinsObjects
{
    public class FilterRemoveColumnDataLessThan: FilterColumnData
    {
        private readonly string nameColumn;
        private readonly int value;

        public FilterRemoveColumnDataLessThan(string nameColumn, int value) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(value), value}
            }
           )
        {
            
            this.nameColumn = nameColumn;
            this.value = value;
        }
       
        public FilterRemoveColumnDataLessThan(CtorDictionary dataNeeded) : base(
            new CtorDictionary(dataNeeded).AddMyValue(
             nameof(Expression), $"{dataNeeded[nameof(nameColumn)]}<{dataNeeded[nameof(value)]}")
            
           )
            
        {
            this.Name = nameof(FilterRemoveColumnDataLessThan);
        }

    }
}