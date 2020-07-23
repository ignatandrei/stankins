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
                if (lastRow < 1)
                    continue;

                //header
                var rowHeader = sheet.GetRow(0);
                if (lastRow == 1)
                {
                    //just the header, no data
                    foreach (var item in rowHeader.Cells)
                    {
                        dtSheet.Columns.Add(item.StringCellValue, typeof(string));

                    }

                }
                else//it has another row, let's see
                {
                    var secondRow = sheet.GetRow(1);
                    var maxCels = secondRow.LastCellNum;
                    for(var i = 0; i < maxCels; i++)
                    {
                        var cell = secondRow.GetCell(i);
                        switch (cell.CellType)
                        {
                            case CellType.Blank:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(string));
                                break;
                            case CellType.Boolean:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(Boolean));
                                break;
                            case CellType.Error:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(string));
                                break;
                            case CellType.Formula:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(string));
                                break;
                            case CellType.Numeric:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(decimal));

                                break;
                            case CellType.String:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(string));

                                break;
                            case CellType.Unknown:
                                dtSheet.Columns.Add(rowHeader.GetCell(i).StringCellValue, typeof(string));

                                break;

                        }
                    }
                    while (dtSheet.Columns.Count < rowHeader.LastCellNum)
                    {
                        dtSheet.Columns.Add(rowHeader.GetCell(dtSheet.Columns.Count).StringCellValue, typeof(string));

                    }
                }

                for (int iRow = 1; iRow < lastRow; iRow++)
                {
                    rowHeader = sheet.GetRow(iRow);
                    var values = rowHeader.Cells.Select(it => it.ToString()).ToList();
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
