using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StankinsDataWeb
{
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IHostingEnvironment hosting;

        public GenericControllerFeatureProvider(IHostingEnvironment hosting)
        {
            this.hosting = hosting;
        }
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var dir = Path.Combine(hosting.ContentRootPath, "definitionHttpEndpoints");
            foreach(var item in Directory.GetFiles(dir))
            {
                
                var ass=LoadFile(item);
                if(ass == null)
                {
                    //TODO:log
                    continue;
                }
                var types= ass.GetExportedTypes();
 
                foreach (var type in types)
                {                    
                   feature.Controllers.Add(type.GetTypeInfo());
                }
            }
        }
        private Assembly LoadFile(string fileName)
        {
            var refs=new List<MetadataReference>();
            var ourRefs=Assembly.GetEntryAssembly().GetReferencedAssemblies();

            foreach(var item in ourRefs)
            {
                var ass=Assembly.Load(item);
                refs.Add(MetadataReference.CreateFromFile(ass.Location));
            }
            refs.Add(MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location));
            //MetadataReference NetStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
            MetadataReference NetStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location);
            refs.Add(NetStandard);
            refs.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location) ); 

            var g=Guid.NewGuid().ToString("N");
            var compilation = CSharpCompilation.Create(g, 
                new[] { CSharpSyntaxTree.ParseText(File.ReadAllText(fileName)) }, 
                refs,
                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                var res = compilation.Emit(ms);
 
                if (!res.Success)
                {           
                    //TODO: log
                    return null;
                }
 
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
                
            }
        }
    }
}
