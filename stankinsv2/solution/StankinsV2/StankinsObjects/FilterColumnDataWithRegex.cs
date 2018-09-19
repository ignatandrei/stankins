using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    public class ChangeColumnName: BaseObject, ITransformer
    {
        public string OldName { get; }
        public string NewName { get; }

        public ChangeColumnName(string oldName, string newName) : this(new CtorDictionary() {
            { nameof(oldName), oldName},
            { nameof(newName), newName}
            }
           )
        {

            
        }
        public ChangeColumnName(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            this.OldName = base.GetMyDataOrThrow<string>(nameof(OldName));
            this.NewName = base.GetMyDataOrThrow<string>(nameof(NewName));

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var tables = base.FindTableAfterColumnName(OldName,receiveData);
            foreach (var item in tables)
            {
                var col = item.Value.Columns[OldName];
                col.ColumnName = NewName;
                
                var tbMeta = receiveData.Metadata.Columns.First(it => it.IDTable == item.Key && it.Name == OldName);
                tbMeta.Name = NewName;

            }

            return receiveData;
        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class FilterColumnDataWithRegex : BaseObject, IFilter
    {
        public string NameColumn { get; }
        public string Expression { get; }

        public FilterColumnDataWithRegex(string nameColumn, string expression) : this(new CtorDictionary() {
            { nameof(nameColumn), nameColumn },
            { nameof(expression), expression}
            }
           )
        {
            NameColumn = nameColumn;
            Expression = expression;
        }
        public FilterColumnDataWithRegex(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
            this.Expression = base.GetMyDataOrThrow<string>(nameof(Expression));
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var reg = new Regex(Expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);            
            var tables = base.FindTableAfterColumnName(NameColumn, receiveData);
            foreach (var item in tables)
            {
                for (int i = item.Value.Rows.Count - 1; i >= 0; i--)
                {
                    var val = item.Value.Rows[i][NameColumn]?.ToString();
                    if (val == null)
                        continue;
                    
                    if (reg.IsMatch(val))
                        continue;

                    item.Value.Rows.RemoveAt(i);

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