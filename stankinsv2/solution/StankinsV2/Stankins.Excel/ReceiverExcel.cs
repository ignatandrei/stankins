using NPOI;
using NPOI.HSSF.Extractor;
using NPOI.SS.UserModel;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Stankins.Excel
{
    public class ReceiverExcel : BaseObject, IReceive
    {
        private readonly string fileName;

        public ReceiverExcel(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));
            this.Name = nameof(ReceiverExcel);
        }
        public ReceiverExcel(string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),fileName}

        })
        {

        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            receiveData ??= new DataToSentTable();
            var dtExcel= new DataTable();
            dtExcel.TableName = Path.GetFileName(fileName);
            IWorkbook wb;
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                wb= WorkbookFactory.Create(file);
            }
            var nrSheets = wb.NumberOfSheets;
            var sheetsTable = new List<DataTable>();
            for (int indexSheet = 0; indexSheet < nrSheets; indexSheet++) 
            {
                var sheet = wb.GetSheetAt(indexSheet);
                var dtSheet = new DataTable();
                sheetsTable.Add(dtSheet);
                dtSheet.TableName = sheet.SheetName;
                var lastRow = sheet.LastRowNum+1;
                
                //header
                var row = sheet.GetRow(0);

                foreach(var item in row.Cells)
                {
                    dtSheet.Columns.Add(item.StringCellValue, typeof(string));

                }

                for (int iRow = 1; iRow < lastRow; iRow++)
                {
                    row = sheet.GetRow(iRow);
                    var values = row.Cells.Select(it => it.ToString()).ToList();
                    var cellMissed = dtSheet.Columns.Count - values.Count();
                    if (cellMissed>0)
                    {
                        values.AddRange(Enumerable.Repeat(" ", cellMissed));
                    }
                    dtSheet.Rows.Add(values.ToArray());
                        
                }
                
            }
            FastAddTables(receiveData, sheetsTable.ToArray());
            return Task.FromResult( receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
