using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Data;
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
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
