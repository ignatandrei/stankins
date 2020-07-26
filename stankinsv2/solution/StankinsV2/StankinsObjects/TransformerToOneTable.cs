using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace StankinsObjects 
{
    
    public class TransformerToOneTable : BaseObject, ITransformer
    {
        private readonly string funcNameTableToBoolean;
        public TransformerToOneTable()
            : this(new CtorDictionary()
        {
                { nameof(funcNameTableToBoolean),null},
        })
        {

        }
        public TransformerToOneTable(string funcNameTableToBoolean) 
            : this(new CtorDictionary()
        {
                { nameof(funcNameTableToBoolean),funcNameTableToBoolean },
        })
        {
            
        }
        public TransformerToOneTable(CtorDictionary dataNeeded) : base(dataNeeded)                  
        {
            this.Name = nameof(TransformerToOneTable);
            this.funcNameTableToBoolean = GetMyDataOrDefault<string>(nameof(funcNameTableToBoolean), null);
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData.DataToBeSentFurther.Count == 0)
                return await Task.FromResult(receiveData) ;

            Func<string, bool> criteria = a => true;
            if (!string.IsNullOrWhiteSpace(funcNameTableToBoolean))
            {
                criteria = await CSharpScript.EvaluateAsync<Func<string, bool>>(funcNameTableToBoolean);

            }

            //var firstTableId =  receiveData.Metadata.Columns.Min(it => it.IDTable);
            var firstTableId = receiveData.DataToBeSentFurther.First(it => criteria(it.Value.TableName)).Key;

            var firstTableColumns = receiveData.Metadata
                .Columns.Where(it => it.IDTable == firstTableId)
                .Select(it => it.Name)
                .ToArray();
            //verify
            


            //var first = receiveData.Metadata.Columns.FirstOrDefault(it => !firstTableColumns.Contains(it.Name));
            //if(first != null)
            //{
            //    throw new ArgumentException($"{string.Join(",", firstTableColumns)} does not contain { first.Name}");
            //}
            var dt = receiveData.DataToBeSentFurther[firstTableId];
            var deleteTables = new List<KeyValuePair<int, DataTable>>();
            foreach (var item in receiveData.DataToBeSentFurther)
            {

                if (item.Key == firstTableId)
                    continue;

                if (criteria(item.Value.TableName))
                {
                    deleteTables.Add(item);
                    dt.Merge(item.Value);
                }
                
            }
            //remove all datatables but not firstTableId
            //while (receiveData.DataToBeSentFurther.Count != 1)
            {
                //var nr = receiveData.DataToBeSentFurther.First();
                //if (nr.Key != firstTableId)
                //    receiveData.DataToBeSentFurther.Remove(nr.Key);
                //nr = receiveData.DataToBeSentFurther.Last();
                //if (nr.Key != firstTableId)
                //    receiveData.DataToBeSentFurther.Remove(nr.Key);
            }

            foreach(var del in deleteTables)
            {
                receiveData.DataToBeSentFurther.Remove(del.Key);
                var iTable = receiveData.Metadata.Tables.First(it => it.Id == del.Key);
                receiveData.Metadata.RemoveTable(iTable);
            }

            //metadata
            var cols = receiveData.Metadata.Columns;
            var tables = receiveData.Metadata.Tables;
            for (int i = tables.Count - 1; i >= 0; i--)
            {
                var t = tables[i];
                if (t.Id == firstTableId)
                    continue;

                //receiveData.Metadata.RemoveTable(t);
                
            }
                
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
