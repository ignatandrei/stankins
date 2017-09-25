using MediaTransform;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace RazorCompile
{    

    public class MediaTransformRazorTuple : MediaTransformString
    {
        public string ContentFileName { get; set; }
        public object O { get; set; }

        public MediaTransformRazorTuple(string contentFileName,object o)
        {
            this.ContentFileName = contentFileName;
            O = o;
        }

        public override async Task Run()
        {
            
            //var contentView = File.ReadAllText(ContentFileName);
            var engine = EngineFactory.CreatePhysical();
            try
            {
                Result = await engine.ParseString<Tuple<object,IRow[]>>(ContentFileName, new Tuple<object, IRow[]>(O, valuesToBeSent));
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //@class.Log(LogLevel.Error, 0, $"razor parse string error {message}", ex, null);
                throw;
            }
        }
    }
        public class MediaTransformRazor : MediaTransformString
    {
        public string ContentFileName { get; set; }
        public MediaTransformRazor(string contentFileName)
        {
            this.ContentFileName = contentFileName;
        }

        public override async Task Run()
        {
            //var contentView = File.ReadAllText(ContentFileName);
            var engine = EngineFactory.CreatePhysical();
            try
            {
                Result= await engine.ParseString<IRow[]>(ContentFileName, valuesToBeSent);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //@class.Log(LogLevel.Error, 0, $"razor parse string error {message}", ex, null);
                throw;
            }
        }
    }

    //public class ConfigureRazor: IRazorRenderer
    //{
    //    public ConfigureRazor()
    //    {
                        
    //    }

    //    public async Task<string> RenderToString<TModel>(string contentView, TModel model)
    //    {
    //        var engine = EngineFactory.CreatePhysical();
    //        try
    //        {
    //            return await engine.ParseString<TModel>(contentView, model);
    //        }
    //        catch (Exception ex)
    //        {
    //            //TODO: log
    //            throw;
    //        }

    //    }
    //}
}
