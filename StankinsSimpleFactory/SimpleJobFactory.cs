using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using ReceiverDB;
using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StankinsSimpleFactory
{
    public class SimpleJobFactory
    {
        ServiceCollection sc;
        Assembly[] assemblies;
        Type[] jobs;
        Type[] receivers;
        Type[] senders;
        Type[] filterTransformers;
        void ConfigureDI()
        {
            sc = new ServiceCollection();
            sc.AddSingleton<ISerializeData>(new SerializeDataOnFile("StankinsData.txt"));
            sc.AddScoped(typeof(DBTableData <, >));

        }
        public SimpleJobFactory()
        {
            ConfigureDI();
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
                    .ToArray()
                    .Where(it => !it.GetTypeInfo().IsAbstract)
                    .ToArray();

            return types.Where(it => !it.GetTypeInfo().IsInterface).ToArray();
        }
        public ISimpleJob  GetJob(string jobName)
        {
            var job = jobs.First(it => it.Name == jobName);
            sc.AddSingleton(typeof(ISimpleJob), job);
            return sc.BuildServiceProvider().GetRequiredService<ISimpleJob>();
            
            //var obj = Activator.CreateInstance(job) as ISimpleJob;
            
        }
        //static Type BaseSubclassOfRawGeneric(Type generic, Type toCheck)
        //{
        //    while (toCheck != null && toCheck != typeof(object))
        //    {
        //        var ti = toCheck.GetTypeInfo();
        //        var cur = ti.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        //        if (generic == cur)
        //        {
        //            return toCheck;
        //        }
        //        toCheck = ti.BaseType;
        //    }
        //    return null;
        //}
        public IReceive GetReceiver(string receiverNameSelected)
        {
            var receiver =receivers.First(it => it.Name == receiverNameSelected);
            //var baseReceiverTable = BaseSubclassOfRawGeneric(typeof(ReceiverTable<,>), receiver);
            //if(baseReceiverTable != null)
            //{
            //    var args = baseReceiverTable.GetGenericArguments();
            //    var t = baseReceiverTable.GetGenericTypeDefinition();
            //    var t1 = t.GetGenericArguments();

            //    var s = "";
                
            //    //return sc.BuildServiceProvider().GetRequiredService(typeof(IReceive<,>)) as IReceive;

            //}
            sc.AddSingleton(typeof(IReceive), receiver);

            var ret= sc.BuildServiceProvider().GetRequiredService<IReceive>();
            var service = sc.First(it => it.ServiceType == typeof(IReceive));
            sc.Remove(service);
            return ret;
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
