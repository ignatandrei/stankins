using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Stankins.AzureDevOps
{
    public class YamlReader : Receiver
    {
        private readonly string fileName;
        private readonly Encoding encoding;

        public YamlReader(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.fileName = GetMyDataOrThrow<string>(nameof(fileName));
            this.encoding = GetMyDataOrDefault(nameof(encoding), Encoding.UTF8);
        }
        public YamlReader(string fileName, Encoding encoding) :this(new CtorDictionary()
        {
            {
                nameof(fileName),fileName
            },
            {
                nameof(encoding),encoding
            }
        })
        {
            this.encoding = encoding;
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var file = new ReadFileToString
            {
                FileEnconding = this.encoding,
                FileToRead = this.fileName
            };

            var data = await file.LoadData();
            var input = new StringReader(data);
            var yaml = new YamlStream();
            yaml.Load(input);
            var node = yaml.Documents[0].RootNode;
            
            return null;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
