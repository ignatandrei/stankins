using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    public abstract class TransformerAddColumnExpression: BaseObject, ITransformer
    {
        public string Expression { get; }
        public string NewColumnName { get; }
        public TransformerAddColumnExpression(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            Expression = base.GetMyDataOrThrow<string>(nameof(Expression));
            NewColumnName = base.GetMyDataOrThrow<string>(nameof(NewColumnName));
           
        }
        public TransformerAddColumnExpression(string expression, string newColumnName) : this(new CtorDictionary()
        {

            { nameof(expression),expression },
            { nameof(newColumnName),newColumnName }

        })
        {

        }
        protected abstract bool IsTableOk(DataTable dt);
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {

            foreach(var item in receiveData.DataToBeSentFurther)
            {
                var table = item.Value;
                if (!this.IsTableOk(table))
                    continue;

                

                table.Columns.Add(NewColumnName + "@", typeof(string), Expression);
                table.Columns.Add(NewColumnName, typeof(string));
                foreach (DataRow dr in table.Rows)
                {
                    dr[NewColumnName] = dr[NewColumnName + "@"]?.ToString();
                }
                table.Columns.Remove(NewColumnName + "@");
                receiveData.Metadata.Columns.Add(new Column() { IDTable = item.Key, Name = NewColumnName, Id = receiveData.Metadata.Columns.Count });
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class TransformerAddColumnExpressionByColumn: TransformerAddColumnExpression
    {
        public string NameColumn { get; }
        public TransformerAddColumnExpressionByColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            NameColumn = base.GetMyDataOrThrow<string>(nameof(NameColumn));
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
        public class TransformerAddColumnExpressionByTable: TransformerAddColumnExpression
    {
        public string NameTable { get; }
        public TransformerAddColumnExpressionByTable(CtorDictionary dataNeeded) : base(dataNeeded)
        {
           
            NameTable = base.GetMyDataOrThrow<string>(nameof(NameTable));
        }
        public TransformerAddColumnExpressionByTable(string nameTable, string expression, string newColumnName) : this(new CtorDictionary()
        {
            { nameof(nameTable),nameTable},
            { nameof(expression),expression },
            { nameof(newColumnName),newColumnName }
               
        })
        {
            
        }

        protected override bool IsTableOk(DataTable dt)
        {
            return string.Equals(dt.TableName, NameTable, StringComparison.CurrentCultureIgnoreCase);                
        }
    }
}
