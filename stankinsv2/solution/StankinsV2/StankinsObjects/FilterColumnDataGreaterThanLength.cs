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
            new CtorDictionary(dataNeeded) {
            { nameof(Expression), $"{dataNeeded[nameof(nameColumn)]}<{dataNeeded[nameof(value)]}"}
            }
           )
            
        {
            this.Name = nameof(FilterRemoveColumnDataLessThan);
        }

    }
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