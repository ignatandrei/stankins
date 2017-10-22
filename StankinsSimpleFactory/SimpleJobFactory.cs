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
using System.Text;

namespace StankinsSimpleFactory
{
    public class SimpleJobFactory
    {
        ServiceCollection sc;
        Assembly[] assemblies;
        Dictionary<Type, Type[]> names;
        void ConfigureDI()
        {
            sc = new ServiceCollection();
            sc.AddSingleton<ISerializeData>(new SerializeDataOnFile("StankinsData.txt"));
            sc.AddTransient(typeof(DBTableData <, >));
            sc.AddSingleton(Encoding.UTF8);
            names = new Dictionary<Type, Type[]>();
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
                if (item == null)
                    continue; 
                var assembly = Assembly.Load(item);
                assembliesList.Add(assembly);
            }
            assemblies = assembliesList.ToArray();
        }
        Type[] Implementation(Type interfaceTo)
        {
            if (interfaceTo.GetTypeInfo().IsInterface)
            {
                var types = assemblies.SelectMany(it => it.ExportedTypes)
                        .Where(it => it.GetInterfaces().Contains(interfaceTo))
                        .ToArray()
                        .Where(it => !it.GetTypeInfo().IsAbstract)
                        .ToArray();

                return types.Where(it => !it.GetTypeInfo().IsInterface).ToArray();

            }
            else
            {
                //is not interface
                return new Type[1] { interfaceTo };
            }
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
       
        
       
        T GetObject<T>(string nameSelected, object[] vals)
        {
            var type = typeof(T);
            return (T)GetObject(type, nameSelected, vals) ;
        }
        public object GetObject(Type type,string nameSelected,  object[] vals)
        {
           
            var obj = Names(type).First(it => it.Name == nameSelected);
            sc.AddSingleton(type, (sp) =>
            {
                return Activator.CreateInstance(obj, vals);
            });

            var ret = sc.BuildServiceProvider().GetRequiredService(type);
            var service = sc.First(it => it.ServiceType == type);
            sc.Remove(service);
            return ret;
        }
        public  object ConstructedObject(Type t)
        {
            try
            {
                var ret = sc.BuildServiceProvider().GetRequiredService(t);
                return ret;
            }
            catch(Exception ex)
            {
                return null;
            }
        }





        public string[] NamesOfObjects(Type t)
        {
            return Names(t).Select(it => it.Name).ToArray();
        }
        
       
        private Type[] Names(Type t)
        {
            if (!names.ContainsKey(t))
            {
                names.Add(t, Implementation(t));
            }
            return names[t];
        }
        ParameterInfo[] GetOwnProperties<T>(string nameSelected)
        {
            //Type t = (Type)x.GetType();
            return GetOwnProperties(typeof(T), nameSelected);
        }
        public ParameterInfo[] GetOwnProperties(Type type,string nameSelected)
        {
            //Type t = (Type)x.GetType();
            var t = Names(type).First(it => it.Name == nameSelected);
            var ctors = t.GetConstructors();
            var max = ctors.Max(it => it.GetParameters().Length);
            var first = ctors.First(it => it.GetParameters().Length == max);
            return first.GetParameters();   
        }
        
    }
}
