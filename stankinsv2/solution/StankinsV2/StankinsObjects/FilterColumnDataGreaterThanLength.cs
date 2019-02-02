using StankinsCommon;

namespace StankinsObjects
{
    public class FilterColumnDataGreaterThanLength : FilterColumnData
    {
        
        public FilterColumnDataGreaterThanLength(string nameColumn, int length) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(Expression), $"Len({nameColumn})>{length}"}
            }
           )
        {
            this.Name = nameof(FilterColumnDataGreaterThanLength);
        }
       
        public FilterColumnDataGreaterThanLength(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
        }

    }
}