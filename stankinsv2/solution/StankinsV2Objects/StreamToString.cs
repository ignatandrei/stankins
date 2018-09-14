using StankinsCommon;
using StankinsV2Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsV2Objects
{
    public class SenderFileCSV: BaseObject, ISender
    {
        public string FolderToSave { get; }
        public SenderFileCSV(string folderToSave) : this(new CtorDictionary()
        {
            { nameof(folderToSave), folderToSave }
        })
        {

        }
        public SenderFileCSV(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FolderToSave = GetMyDataOrDefault<string>(nameof(FolderToSave), Environment.CurrentDirectory);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            Regex illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);
        
            var sender = new StreamToString();
            var names = receiveData.DataToBeSentFurther
                .Select(it => it.Value.TableName)
                .Select(it=> illegalInFileName.Replace(it, "_"))
                .ToArray();
            int i = 0;
            foreach(var item in sender.StreamTo(receiveData))
            {

                string nameFile = Path.Combine(FolderToSave, names[i]);

                File.WriteAllText(nameFile, item);
                i++;

            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }

    public class StreamToString: IStreaming<String>
    {

        public IEnumerable<string> StreamTo(IDataToSent dataToSent)
        {
            foreach(var dt in dataToSent.DataToBeSentFurther)
            {
                var sb = new StringBuilder();
                var cols= new List<string>();
                foreach (DataColumn dc in dt.Value.Columns)
                {
                    if (dc.DataType == typeof(string))
                    {
                        cols.Add(dc.ColumnName);
                    }
                }
                sb.AppendLine(string.Join(",", cols.ToArray()));
                foreach (DataRow dr in dt.Value.Rows)
                {
                    sb.AppendLine(string.Join(",", dr.ItemArray));
                }
                yield return sb.ToString();

            }
        }
    }
}
