using RazorLight;
using Stankins.Interfaces;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SenderInterpretedRazor
{
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
        public Task<bool> Initialize()
        {
            engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(this.GetType())
            .UseMemoryCachingProvider()
            .Build();
            
            //await engine.CompileRenderAsync< DataTable>((key,Content,new DataTable());
            //var found = engine.TemplateCache.RetrieveTemplate(key);
            //return found.Success;
            return Task.FromResult( true);
        }

        public  IEnumerable<string> StreamTo(IDataToSent dataToSent)
        {
            
            //var found = engine.TemplateCache.RetrieveTemplate(key);
            //if (!found.Success)
            //{
            //    throw new ArgumentException(" the template was not initialized");
            //}
            foreach (var item in dataToSent.DataToBeSentFurther)
            {
                
                var res = engine.CompileRenderStringAsync(key,Content, item.Value).ConfigureAwait(false).GetAwaiter().GetResult();

                yield return res;
            }
        }
    }
}
