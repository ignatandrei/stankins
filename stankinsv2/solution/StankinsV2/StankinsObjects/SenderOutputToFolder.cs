using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;

namespace StankinsObjects
{
    public class SenderOutputToFolder:BaseObject, ISender
    {
        private readonly bool addKey;
        public string FolderToSave { get; }
        public SenderOutputToFolder(string folderToSave,bool addKey) : this(new CtorDictionary()
        {
            { nameof(folderToSave), folderToSave },
            {nameof(addKey),addKey }
        })
        {
            
        }
        public SenderOutputToFolder(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            FolderToSave = GetMyDataOrDefault<string>(nameof(FolderToSave), Environment.CurrentDirectory);
            addKey = GetMyDataOrDefault<bool>(nameof(addKey), true);
            Name = nameof(SenderOutputToFolder);
        }
        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if (receiveData == null)
                return receiveData;
            if ((!string.IsNullOrWhiteSpace(FolderToSave)) && (!Directory.Exists(FolderToSave)))
            {
                Directory.CreateDirectory(FolderToSave);
            }
            Regex illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);
            DataTable output=null;
            try
            {
                output = receiveData.FindAfterName("OutputString").Value;
            }
            catch
            {
                //do nothing
            }

            if (output != null)
            {
                foreach (DataRow outputRow in output.Rows)
                {
                    string nameFile =  $"{outputRow["Name"]}";
                    if (addKey)
                    {
                        nameFile= $"{outputRow["ID"]}_" + nameFile;
                    }
                    nameFile = illegalInFileName.Replace(nameFile, "_");
                    nameFile = Path.Combine(FolderToSave, nameFile);
                    
                    File.WriteAllText(nameFile, outputRow["Contents"]?.ToString());
                }
            }
            try
            {
                output = receiveData.FindAfterName("OutputByte").Value;
            }
            catch
            {
                //do nothing
            }

            if (output != null)
            {
                foreach (DataRow outputRow in output.Rows)
                {
                    string nameFile = $"{outputRow["Name"]}";
                    if (addKey)
                    {
                        nameFile = $"{outputRow["ID"]}_" + nameFile;
                    }
                    nameFile = illegalInFileName.Replace(nameFile, "_");
                    nameFile = Path.Combine(FolderToSave, nameFile);
                    File.WriteAllBytes(nameFile,(byte[]) outputRow["Contents"]);
                }
            }


            return await Task.FromResult(receiveData);
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
