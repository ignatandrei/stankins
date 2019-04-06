using StankinsCommon;

namespace StankinsObjects
{
    public class FilterRemoveColumnDataGreaterThanLength : FilterColumnData
    {
        
        public FilterRemoveColumnDataGreaterThanLength(string nameColumn, int length) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(Expression), $"Len({nameColumn})>{length}"}
            }
           )
        {
            this.Name = nameof(FilterRemoveColumnDataGreaterThanLength);
        }
       
        public FilterRemoveColumnDataGreaterThanLength(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
        }

    }
}