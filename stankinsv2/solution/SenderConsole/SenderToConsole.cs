using ConsoleTables;
using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SenderConsole
{
    public class SenderToConsole : BaseObject, ISender
    {
        public SenderToConsole():base(null)
        {

        }
        public SenderToConsole(CtorDictionary dataNeeded) :base(null)
        {

        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            Console.WriteLine($"--->start Tables");
            foreach (var item in receiveData.DataToBeSentFurther)
            {
                var dt = item.Value;
                var cols = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    cols[i] = dt.Columns[i].ColumnName.Trim();
                }
                var table = new ConsoleTable(cols);
                foreach(DataRow dr in dt.Rows)
                {
                    table.AddRow(dr.ItemArray);
                }
                Console.WriteLine($"Table {dt.TableName}");
                table.Write(Format.Alternative);
            }

            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
