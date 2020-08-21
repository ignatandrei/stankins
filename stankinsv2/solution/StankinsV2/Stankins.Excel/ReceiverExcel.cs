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
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

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
       public Type FromCellType(CellType type )
        {
            
            switch (type)
            {
                case CellType.Blank:
                    return typeof(string);
                    
                case CellType.Boolean:
                    return typeof(Boolean);
                    
                case CellType.Error:
                    return  typeof(string);
                    
                case CellType.Formula:
                    throw new ArgumentException("need cell");

                case CellType.Numeric:
                    return  typeof(decimal);

                    
                case CellType.String:
                    return  typeof(string);

                    
                case CellType.Unknown:
                    return  typeof(string);

                default:
                    throw new ArgumentException($"do not understand {type}");
            }
        }
        public object ValueCell(ICell cell, CellType type)
        {
            switch (type)
            {
                case CellType.Blank:
                    return null;

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                case CellType.Error:
                    return "ERROR";

                case CellType.Formula:
                    return ValueCell(cell, cell.CachedFormulaResultType);

                case CellType.Numeric:
                    return cell.NumericCellValue;


                case CellType.String:
                    return cell.StringCellValue;


                case CellType.Unknown:
                    return null;


                default:
                    throw new ArgumentException($"cannot find cell value for ${type}");
            }
        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            receiveData ??= new DataToSentTable();
            var dtExcel= new DataTable();
            dtExcel.TableName = Path.GetFileName(fileName);
            dtExcel.Columns.Add("Number", typeof(int));
            dtExcel.Columns.Add("SheetName", typeof(string));

            FastAddTable(receiveData, dtExcel);
            IWorkbook wb;

            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                wb= WorkbookFactory.Create(file);
            }
            if (wb is XSSFWorkbook)
            {
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            }
            else
            {
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);
            }
            var nrSheets = wb.NumberOfSheets;
            var sheetsTable = new List<DataTable>();
            for (int indexSheet = 0; indexSheet < nrSheets; indexSheet++) 
            {
                var sheet = wb.GetSheetAt(indexSheet);
                var dtSheet = new DataTable();
                sheetsTable.Add(dtSheet);
                dtSheet.TableName = sheet.SheetName;

                dtExcel.Rows.Add(new object[2] { indexSheet, sheet.SheetName });

                var lastRow = sheet.LastRowNum+1;
                if (lastRow < 1)
                    continue;

                //header
                var rowHeader = sheet.GetRow(0);
                if (rowHeader == null)
                    continue;
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
                    for (var i = 0; i < maxCels; i++)
                    {
                        var cell = secondRow.GetCell(i);
                        string name = (rowHeader.GetCell(i)?.ToString())??"";
                        if (string.IsNullOrEmpty(name))//no empty headers
                            break;
                        Type t;
                        switch (cell.CellType)
                        {
                            case CellType.Formula:
                                t = FromCellType(cell.CachedFormulaResultType);
                                break;
                            default:
                                t = FromCellType(cell.CellType);
                                break;
                        }
                        dtSheet.Columns.Add(name, t);

                    }
                    while (dtSheet.Columns.Count < rowHeader.LastCellNum)
                    {
                        dtSheet.Columns.Add(rowHeader.GetCell(dtSheet.Columns.Count).StringCellValue, typeof(string));

                    }
                }
                var nrCols = dtSheet.Columns.Count;
                for (int iRow = 1; iRow < lastRow; iRow++)
                {
                    rowHeader = sheet.GetRow(iRow);
                    var values = rowHeader.Cells.Select(it =>ValueCell( it, it.CellType)).ToList();
                    var cellMissed = nrCols- values.Count();
                    if (cellMissed>0)
                    {//found row with less values than the header
                        while(values.Count< nrCols)
                        {
                            values.Add(null);
                        }
                    }
                    if (cellMissed < 0)
                    {//found row with more values than the header
                        while(!(values.Count == nrCols))
                        {
                            values.RemoveAt(values.Count - 1);
                        }
                    }
                    try
                    {
                        bool canAdd = true;
                        for (int i = 0; i < values.Count; i++)
                        {
                            var currentType = dtSheet.Columns[i].DataType;
                            
                            if(! CanChangeTypeToDataColumn(values[i],currentType))
                            {
                                ConvertColumnType(dtSheet, dtSheet.Columns[i].ColumnName, typeof(string));
                                
                                currentType = dtSheet.Columns[i].DataType;
                                if (!CanChangeTypeToDataColumn(values[i], currentType))
                                {

                                    string message = ($"cannot add row {iRow} because {values[i]} do not match {dtSheet.Columns[i].DataType}");
                                    Console.WriteLine(message);
                                    canAdd = false;
                                }
                            }
                        }
                        if(canAdd)
                        dtSheet.Rows.Add(values.ToArray());
                    }
                    catch(Exception ex)
                    {
                        string s = ex.Message;
                        throw;
                    }
                    
                }
                
            }
            FastAddTables(receiveData, sheetsTable.ToArray());
            return Task.FromResult( receiveData);
        }

        public static void ConvertColumnType(DataTable dt, string columnName, Type newType)
        {
            string newName = columnName + Guid.NewGuid().ToString("N");
            using (DataColumn dc = new DataColumn(newName, newType))
            {
                int ordinal = dt.Columns[columnName].Ordinal;
                dt.Columns.Add(dc);
                dc.SetOrdinal(ordinal);
                
                foreach (DataRow dr in dt.Rows)
                {
                    dr[newName] =(dr[columnName] == DBNull.Value) ? DBNull.Value:  Convert.ChangeType(dr[columnName], newType);
                }
                // Remove the old column
                dt.Columns.Remove(columnName);

                // Give the new column the old column's name
                dc.ColumnName = columnName;
            }
        }
        static bool CanChangeTypeToDataColumn(object value, Type toType)
        {
            try
            {
                if (value == null)
                    return true;
                var obj = Convert.ChangeType(value, toType);
                return true;
            }
            catch
            {
                return false;
            }
        } 
        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
