using StankinsCommon;
using Stankins.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StankinsObjects ;

namespace Stankins.File
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

            var sender = new StreamToStringCSV();
            var names = receiveData.DataToBeSentFurther
                .Select(it => it.Value.TableName)
                .Select(it=> illegalInFileName.Replace(it, "_"))
                .ToArray();
            int i = 0;
            foreach (var item in sender.StreamTo(receiveData))
            {

                string nameFile = Path.Combine(FolderToSave, names[i]);

                System.IO.File.WriteAllText(nameFile, item);
                i++;

            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
