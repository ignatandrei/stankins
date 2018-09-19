using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    public class FilterColumnDataGreaterThanLength : FilterColumnData
    {
        
        public FilterColumnDataGreaterThanLength(string nameColumn, int length) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(Expression), $"Len({nameColumn})>{length}"}
            }
           )
        {
            
        }
        public FilterColumnDataGreaterThanLength(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
        }

    }
    public class FilterColumnData : BaseObject, IFilter
    {
        public string NameColumn { get; }
        public string Expression { get; }

        public FilterColumnData(string nameColumn, string expression) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(expression), expression}
            }
           )
        {
            NameColumn = nameColumn;
            Expression = expression;
        }
        public FilterColumnData(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
            this.Expression = base.GetMyDataOrThrow<string>(nameof(Expression));
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = base.FindTableAfterColumnName(NameColumn, receiveData);
            foreach (var item in tables)
            {
                var dv = new DataView(item.Value)
                {
                    RowFilter = Expression
                };
                foreach (DataRowView row in dv)
                {
                    row.Delete();
                }
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}