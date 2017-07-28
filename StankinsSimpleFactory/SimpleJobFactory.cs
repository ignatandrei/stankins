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
        Type[] senders;
        Type[] filterTransformers;
        public SimpleJobFactory()
        {
            var assembliesList = new List<Assembly>();
            //todo : replace with plugins
            //assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId).ToArray();
            foreach (var item in assemblyNames)
            {
                var assembly = Assembly.Load(item);
                assembliesList.Add(assembly);
            }
            assemblies = assembliesList.ToArray();
        }
        Type[] Implementation(Type interfaceTo)
        {
            var types = assemblies.SelectMany(it => it.ExportedTypes)
                    .Where(it => it.GetInterfaces().Contains(interfaceTo))
                    .ToArray();

            return types.Where(it => !it.GetTypeInfo().IsInterface).ToArray();
        }
        public ISimpleJob  GetJob(string jobName)
        {
            var job = jobs.First(it => it.Name == jobName);
            var obj = Activator.CreateInstance(job) as ISimpleJob;
            return obj;
        }
        
        public IReceive GetReceiver(string receiverNameSelected)
        {
            var receiver =receivers.First(it => it.Name == receiverNameSelected);
            var IReceiver = Activator.CreateInstance(receiver, true);
            return receiver as IReceive;
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
            if (receivers == null)
            {
                receivers = Implementation(typeof(IReceive));
            }
            return receivers.Select(it => it.Name).ToArray();
        }
        public string[] SenderNames()
        {
            if (senders == null)
            {
                senders = Implementation(typeof(ISend));
            }
            return senders.Select(it => it.Name).ToArray();
        }
        public string[] FilterTransformerNames()
        {
            if (filterTransformers == null)
            {
                filterTransformers = Implementation(typeof(IFilterTransformer));
            }
            return filterTransformers.Select(it => it.Name).ToArray();
        }

        
    }
}
