using RazorLight;
using StankinsCommon;
using StankinsV2Interfaces;
using StankinsV2Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SenderInterpretedRazor
{
    //TODO: abstract writing files to folder
    public class SenderRazorTableOneByOne : BaseObject, ISender
    {
        public string ContentFile { get; }
        public string FolderName { get; }

        public SenderRazorTableOneByOne(string contentFile, string folderName) : this(new CtorDictionary() {
            { nameof(contentFile), contentFile},
            { nameof(folderName), folderName}
            }
          )
        {
            
        }
        public SenderRazorTableOneByOne(CtorDictionary dataNeeded) : base(dataNeeded)
        {

            this.ContentFile = base.GetMyDataOrThrow<string>(nameof(ContentFile));
            this.FolderName =  base.GetMyDataOrThrow<string>(nameof(FolderName));

        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var razor = new StreamRazorTableOneByOne(ContentFile);
            var res= await razor.Initialize();
            if (!res)
            {
                //TODO: make csutom exception here
                throw new ArgumentException("cannot initialize razor");
            }
            //TODO: this will be used more than once - put some utilities
            var illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);

            
            var names = receiveData.DataToBeSentFurther
                .Select(it => it.Value.TableName)
                .Select(it => illegalInFileName.Replace(it, "_"))
                .ToArray();
            int i = 0;
            foreach (var data in razor.StreamTo(receiveData))
            {
                string nameFile = Path.Combine(FolderName, names[i]);
                File.WriteAllText(nameFile, data);
                i++;
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
    public class StreamRazorTableOneByOne : IStreaming<String>
    {
        public StreamRazorTableOneByOne(string content)
        {
            Content = content;
            key = Guid.NewGuid().ToString();
        }

        public string Content { get; }
        IRazorLightEngine engine;
        readonly string key;
        public async Task<bool> Initialize()
        {
            engine = new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build();
            
            await engine.CompileRenderAsync< DataTable>(key,Content,new DataTable());
            var found = engine.TemplateCache.RetrieveTemplate(key);
            return found.Success;
            //return true;
        }

        public IEnumerable<string> StreamTo(IDataToSent dataToSent)
        {
            
            var found = engine.TemplateCache.RetrieveTemplate(key);
            if (!found.Success)
            {
                throw new ArgumentException(" the template was not initialized");
            }
            foreach (var item in dataToSent.DataToBeSentFurther)
            {
                
                var res = engine.RenderTemplateAsync(found.Template.TemplatePageFactory(), item.Value).ConfigureAwait(false).GetAwaiter().GetResult();

                yield return res;
            }
        }
    }
}
