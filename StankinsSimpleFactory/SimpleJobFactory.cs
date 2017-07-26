using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StankinsSimpleFactory
{
    public class SimpleJobFactory
    {
        Assembly[] assemblies;
        Type[] jobs;
        Type[] receivers;
        public SimpleJobFactory()
        {
            var assembliesList=new List<Assembly>();
            //todo : replace with plugins
            //assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId).ToArray();
            foreach(var item in assemblyNames)
            {
                var assembly = Assembly.Load(item);
                assembliesList.Add(assembly);
            }
            assemblies = assembliesList.ToArray();
        }
        Type[] Implementation(Type interfaceTo)
        {
            var types= assemblies.SelectMany(it => it.ExportedTypes)
                    .Where(it => it.GetInterfaces().Contains(interfaceTo))                    
                    .ToArray();

            return types.Where(it => !it.GetTypeInfo().IsInterface).ToArray();
        }
        public string[] JobNames()
        {
            if (jobs == null) {
                jobs = Implementation(typeof(IJob));                
                }
            return jobs.Select(it => it.Name).ToArray();
        }

        public string[] ReceiverNames()
        {
            if(receivers == null)
            {
                receivers = Implementation(typeof(IReceive));
            }
            return receivers.Select(it => it.Name).ToArray();
        }
    }
}
