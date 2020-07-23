using Newtonsoft.Json;
using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public class SenderDefinitionToFile : BaseObject, ISender
    {

        private readonly string fileName;

        public SenderDefinitionToFile(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));
            this.Name = nameof(SenderDefinitionToFile);
        }
        public SenderDefinitionToFile(string fileName) : this(new CtorDictionary()
        {
            {nameof(fileName),fileName}

        })
        {

        }
        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            if ((receiveData?.DataToBeSentFurther?.Count ?? 0) == 0)
                return Task.FromResult(receiveData);
            var SB = new StringBuilder("[");

            var arr = receiveData.DataToBeSentFurther.Select(it =>
            new {
                name = it.Key,
                columns = it.Value.Columns
            })
                .ToArray();
            var text = JsonConvert.SerializeObject(arr);
            File.WriteAllText(fileName, text);
            return Task.FromResult(receiveData);

        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
