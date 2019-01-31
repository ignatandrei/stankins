using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Stankins.Interfaces;
using StankinsCommon;

using StankinsObjects ;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stankins.Office
{
    public class SenderExcel : BaseObjectInSerial<SenderOutputExcel,SenderOutputToFolder>
    {
        public SenderExcel(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            


        }
        public SenderExcel(string fileName) : this(new CtorDictionary()
        {

            { nameof(fileName),fileName},
            {"addKey",false }
        })
        {

        }

        


    }
    public class SenderOutputExcel : BaseObjectSender, ISender
    {
        public string FileName { get; }
        public SenderOutputExcel(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FileName = base.GetMyDataOrThrow<string>(nameof(FileName));


        }
        public SenderOutputExcel(string fileName) : this(new CtorDictionary()
        {

            { nameof(FileName),fileName},
        })
        {

        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);
            IWorkbook workbook = new XSSFWorkbook();
            int nrSheets = 0;
            foreach (var kv in receiveData.DataToBeSentFurther)
            {
                var dt = kv.Value;  
                var sheet = workbook.CreateSheet($"Sheet{nrSheets++}" + illegalInFileName.Replace(dt.TableName, "_"));
                int rowIndex = 0;
                var row = sheet.CreateRow(rowIndex);
                int celIndex = 0;
                foreach (DataColumn c in dt.Columns)
                {

                    row.CreateCell(celIndex++).SetCellValue(c.ColumnName);
                }
                foreach (DataRow dr in dt.Rows)
                {

                    row = sheet.CreateRow(++rowIndex);
                    celIndex = 0;
                    foreach (var item in dr.ItemArray)
                    {

                        row.CreateCell(celIndex++).SetCellValue(item?.ToString());
                    }

                }
            }
            this.CreateOutputIfNotExists(receiveData);
            using (var fs = new MemoryStream())
            {
                workbook.Write(fs);
                this.OutputByte.Rows.Add(null, this.FileName, fs.ToArray());
            }
            

            

            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
