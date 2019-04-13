using StankinsCommon;
namespace StankinsObjects
{
    public class FilterRetainColumnDataContains: FilterColumnData
    { 
        private readonly string nameColumn;
        private readonly string stringToHave;

        public FilterRetainColumnDataContains(string nameColumn, string stringToHave) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(stringToHave), stringToHave }
            }
          )
        { 
            this.nameColumn = nameColumn;
            this.stringToHave = stringToHave;
            
        }
        public FilterRetainColumnDataContains(CtorDictionary dataNeeded) : base(
            new CtorDictionary(dataNeeded)
            .AddMyValue(nameof(Expression), $"not({dataNeeded[nameof(nameColumn)]} like '%{dataNeeded[nameof(stringToHave)]}%')")
            )
        {
            this.Name = nameof(FilterRetainColumnDataContains);
        }

    }
}