using MediaTransform;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using RazorLight;
using RazorLight.Extensions;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RazorCompile
{
    public class MediaTransformRazor : MediaTransformString
    {
        public string ContentFileName { get; set; }
        public MediaTransformRazor(string contentFileName)
        {
            this.ContentFileName = contentFileName;
        }

        public override async Task Run()
        {
            var contentView = File.ReadAllText(ContentFileName);
            var engine = EngineFactory.CreatePhysical(AppContext.BaseDirectory);
            try
            {
                Result= engine.ParseString<IRow[]>(contentView, valuesToBeSent);
            }
            catch (Exception ex)
            {
                //TODO: log
                throw;
            }
        }
    }

    public class ConfigureRazor: IRazorRenderer
    {
        public ConfigureRazor()
        {
                        
        }

        public async Task<string> RenderToString<TModel>(string contentView, TModel model)
        {
            var engine = EngineFactory.CreatePhysical(AppContext.BaseDirectory);
            try
            {
                return engine.ParseString<TModel>(contentView, model);
            }
            catch (Exception ex)
            {
                //TODO: log
                throw;
            }

        }
    }
}
