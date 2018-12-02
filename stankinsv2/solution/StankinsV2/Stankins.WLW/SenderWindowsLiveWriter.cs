using OpenMcdf;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.WLW
{
    public class SenderWindowsLiveWriter : BaseObject, ISender
    {
        public SenderWindowsLiveWriter(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FolderPath = base.GetMyDataOrDefault<string>(nameof(FolderPath),Environment.CurrentDirectory);
            SeparatorColumn = base.GetMyDataOrDefault<string>(nameof(SeparatorColumn), ",");
            SeparatorRow = base.GetMyDataOrDefault<string>(nameof(SeparatorRow), Environment.NewLine);
            First = base.GetMyDataOrDefault<string>(nameof(First), "");
            Last = base.GetMyDataOrDefault<string>(nameof(Last), "");


        }
        public SenderWindowsLiveWriter(string folderPath,string separatorRow,string separatorColumn, string first, string last) : this(new CtorDictionary()
        {

            { nameof(folderPath),folderPath},
            { nameof(separatorRow),separatorRow},
            { nameof(separatorColumn),separatorColumn},
            { nameof(first),first},
            { nameof(last),last},


        })
        {

        }
        static byte[] Str(string text)
        {
            return Encoding.Unicode.GetBytes(text);
        }


        public string FolderPath { get; }
        public string SeparatorRow { get; set; }
        public string SeparatorColumn { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var nr = receiveData.DataToBeSentFurther.Count;
            var fmt = "#".PadLeft(nr.ToString().Count()+1).Replace(" ","0");
            foreach (var item in receiveData.DataToBeSentFurther)
            {
                
                string fileName = item.Key.ToString (fmt) + item.Value.TableName;
                var fullFilePath= Path.Combine(FolderPath, fileName + ".wpost");
                var cf = new CompoundFile(@"a.wpost", CFSUpdateMode.ReadOnly, CFSConfiguration.Default);

                var rs = cf.RootStorage;
                var t = rs.TryGetStream("Title");

                t.SetData(Str(fileName));
                var allData = new StringBuilder(First);
                var c = rs.TryGetStream("Contents");
                foreach(DataRow dr in item.Value.Rows)
                {
                    var str = string.Join(SeparatorColumn, dr.ItemArray);
                    allData.Append($"{str}{SeparatorRow}");
                }
                allData.Append(Last);
                c.SetData(Str(allData.ToString()));
                cf.Save(fullFilePath);
                
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
