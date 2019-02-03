using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class TransformerOutputStringColumnName: TransformerUpdateColumn
    {
        public TransformerOutputStringColumnName(string Expression) : this(new CtorDictionary()
        {
            { nameof(nameColumn), "Name" },
             { nameof(nameTable), "OutputString" },
            {nameof(Expression),Expression },
            {nameof(NewColumnName),Guid.NewGuid().ToString("N") }
        })
        {

        }
        public TransformerOutputStringColumnName(CtorDictionary dataNeeded) : base(dataNeeded)
        {
          
            Name = nameof(TransformerOutputStringColumnName);

        }

    }
    public class TransformerUpdateColumn : TransformerAddColumnExpression, ITransformer
    {
        
        public string nameColumn { get; }
        public string nameTable { get; }
        public TransformerUpdateColumn(string nameColumn,string nameTable,string Expression ) : this(new CtorDictionary()
        {
            { nameof(nameColumn), nameColumn },
             { nameof(nameTable), nameTable },
            {nameof(Expression),Expression },
            {nameof(NewColumnName),Guid.NewGuid().ToString("N") }
        })
        {

        }
        public TransformerUpdateColumn(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.nameColumn = GetMyDataOrThrow<string>(nameof(nameColumn));
            this.nameTable = GetMyDataOrThrow<string>(nameof(nameTable));
            Name = nameof(TransformerUpdateColumn);

        }
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }

        public override async  Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            receiveData = await base.TransformData(receiveData);
            foreach (var item in receiveData.DataToBeSentFurther)
            {
                var table = item.Value;
                if (!this.IsTableOk(table))
                    continue;

                table.Columns.Remove(nameColumn);
                table.Columns[NewColumnName].ColumnName = nameColumn;
                var col=receiveData.Metadata.Columns.First(c=>c.Name==NewColumnName);
                receiveData.Metadata.Columns.Remove(col);
            }
            return receiveData;

        }
        protected override bool IsTableOk(DataTable dt)
        {
            return dt.TableName == nameTable;
        }
    }
}
