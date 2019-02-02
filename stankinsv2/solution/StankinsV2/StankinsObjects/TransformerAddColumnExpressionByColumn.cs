using StankinsCommon;
using Stankins.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformerAddColumnExpressionByColumn: TransformerAddColumnExpression
    {
        public string NameColumn { get; }
        public TransformerAddColumnExpressionByColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
            Name = nameof(TransformerAddColumnExpressionByColumn);
        }
        public TransformerAddColumnExpressionByColumn(string nameColumn, string expression, string newColumnName) : this(new CtorDictionary()
        {
            {nameof(nameColumn),nameColumn },
            { nameof(expression),expression },
            { nameof(newColumnName),newColumnName }

        })
        {

        }
        string[] tables;
        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            tables = FindTableAfterColumnName(NameColumn, receiveData)
                .Select(it=>it.Value.TableName)
                .ToArray();
            return base.TransformData(receiveData);
        }
        protected override bool IsTableOk(DataTable dt)
        {
            return tables.Contains(dt.TableName);
        }
    }
}
