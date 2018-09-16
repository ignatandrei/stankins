using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TransformerGrouping
{
    public class SeparateByNumber : BaseObject,ITransformer
    {
        public SeparateByNumber(CtorDictionary dict):base(dict)
        {
            this.Name = "separate by number";
            this.NameTable = base.GetMyDataOrThrow<string>(nameof(NameTable));
            this.Number = base.GetMyDataOrThrow<int>(nameof(Number));
        }
        public SeparateByNumber(string nameTable, int number) : this(
            new CtorDictionary()
            {
                {nameof(nameTable),nameTable },
                {nameof(number),number }
            })
        {
         
        }
        
        public string NameTable { get; }
        public int Number { get; }


        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            
            var data = receiveData.FindAfterName(NameTable);
            var dt = data.Clone();
            var maxRows = (data.Rows.Count);
            //add latest rows - TODO: test
            var max = (maxRows / Number)*Number + 1;
            
            for (var i = 0; i <max ; i++)
            {
                
                if ((i+1) % Number == 0)
                {
                    dt.TableName = data.TableName + $"{i+1-Number}_{i + 1}";
                    var id= receiveData.AddNewTable(dt);
                    receiveData.Metadata.AddTable(dt,id);
                    dt = data.Clone();
                    
                }
                else
                {
                    if(i<maxRows)
                        dt.ImportRow(data.Rows[i]);
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
