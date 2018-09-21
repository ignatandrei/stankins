using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StankinsObjects
{
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
