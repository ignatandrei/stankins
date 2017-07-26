using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;
using RazorLight;
using RazorLight.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RazorCompile
{

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
