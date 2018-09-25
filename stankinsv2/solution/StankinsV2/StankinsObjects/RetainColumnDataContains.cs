using StankinsCommon;

namespace StankinsObjects
{
    public class RetainColumnDataContains: FilterColumnData
    {
        public RetainColumnDataContains(string nameColumn, string stringToHave) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(Expression), $"not({nameColumn} like '%{stringToHave}%')" }
            }
          )
        {
            
        }
        public RetainColumnDataContains(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
        }

    }
}