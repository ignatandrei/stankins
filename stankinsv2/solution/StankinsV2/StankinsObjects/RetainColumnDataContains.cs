using StankinsCommon;

namespace StankinsObjects
{
    public class FilterRetainColumnDataContains: FilterColumnData
    {
        public FilterRetainColumnDataContains(string nameColumn, string stringToHave) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(Expression), $"not({nameColumn} like '%{stringToHave}%')" }
            }
          )
        {
            
        }
        public FilterRetainColumnDataContains(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
        }

    }
}