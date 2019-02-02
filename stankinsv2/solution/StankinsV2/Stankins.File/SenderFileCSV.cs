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
    public class SenderAllTablesToFileCSV: BaseObject, ISender
    {
        public string FolderToSave { get; }
        public SenderAllTablesToFileCSV(string folderToSave) : this(new CtorDictionary()
        {
            { nameof(folderToSave), folderToSave }
        })
        {

        }
        public SenderAllTablesToFileCSV(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FolderToSave = GetMyDataOrDefault<string>(nameof(FolderToSave), Environment.CurrentDirectory);
            Name = nameof(SenderAllTablesToFileCSV);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            Regex illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);

            var sender = new StreamToStringCSV();
            var names = receiveData.DataToBeSentFurther
                .Select(it => it.Value.TableName)
                .Select(it=> illegalInFileName.Replace(it, "_"))
                .Select(it=>string.IsNullOrWhiteSpace(it)?Guid.NewGuid().ToString("N"):it)
                .Select(it=>it+".csv")
                .ToArray();
            int i = 0;
            if (!string.IsNullOrWhiteSpace(FolderToSave) && !Directory.Exists(FolderToSave))
                Directory.CreateDirectory(FolderToSave);

            foreach (var item in sender.StreamTo(receiveData))
            {

                string nameFile = Path.Combine(FolderToSave, names[i]);

                System.IO.File.WriteAllText(nameFile, item);
                i++;

            }
            return await Task.FromResult(receiveData) ;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
