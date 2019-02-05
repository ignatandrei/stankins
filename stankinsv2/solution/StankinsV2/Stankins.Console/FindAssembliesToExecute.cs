using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stankins.Console
{
    class FindAssembliesToExecute
    {
        static List<string> verifiedAssembly = new List<string>();
        public IEnumerable<KeyValuePair<Type, CtorDictionary>> FindNamesToBeExecuted()
        {
            return VerifyName(Assembly.GetExecutingAssembly());
        }

        private IEnumerable<KeyValuePair<Type, CtorDictionary>> VerifyName(Assembly a)
        {
            if (verifiedAssembly.Contains(a.FullName))
            {
                yield break;
            }

            verifiedAssembly.Add(a.FullName);

            foreach(var item in VerifyTypes(a)){
                yield return item;
            }

            foreach (var ass in a.GetReferencedAssemblies())
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.Load(ass);

                }
                catch (FileNotFoundException)
                {
                    continue;
                }
                foreach (var item in VerifyName(assembly))
                {
                    yield return item;
                }
            }
            yield break;
        }

        private IEnumerable<KeyValuePair< Type,CtorDictionary>> VerifyTypes(Assembly a)
        {
            if (!a.FullName.Contains("tankins"))
                yield break;

            var interf = typeof(IBaseObject);
            var types = a.GetTypes();
            foreach (var t in types)
            {
                if (!t.IsClass || t.IsAbstract)
                {
                    continue;
                }
                if (!interf.IsAssignableFrom(t))
                {
                    continue;
                }
                if (t.ContainsGenericParameters)
                {
                    continue;
                }
                //if (new[] { "FilterColumnDataGreaterThanLength"
                //    ,"FilterRetainColumnDataContains"
                //    ,"ReceiverDBSqlServer"
                //    ,"ReceiveDatabasesSql"
                //    //,"ReceiveMetadataFromDatabaseSql"
                //    ,"ReceiveTableDatabaseSql"
                //    ,"ReceiveQueryFromDatabaseSql"
                //    ,"DBReceiverStatement"
                //}.Contains(t.Name))
                //{
                //    continue;
                //}
                var x = t.FullName;
                var s = x.Contains("FilterColumnDataGreaterThanLength");
                var obj = TryToConstruct(t) as CtorDictionary;
                if (obj != null)
                {
                    //TODO: investigate why null
                    yield return new KeyValuePair<Type, CtorDictionary>(t, obj);
                }
                

            }
            CtorDictionary TryToConstruct(Type t)
            {
                var c = new CtorDictionary();
                while (true)
                {
                    try
                    {
                        var res = Activator.CreateInstance(t, c);
                        return c;
                    }
                    catch (TargetInvocationException tex)
                    {
                        var lenArgs = c.Count;
                        var ex = tex.InnerException as ArgumentException;
                        if (ex == null)
                        {
                            throw new Exception($"for {t.Name} tex.InnerException is {tex.InnerException} ");
                        }
                        var name = ex.ParamName;
                        if (c.ContainsKey(name))
                        {
                            throw new Exception($"type {t} has {name} twice");
                        }
                        foreach (var ctor in t.GetConstructors())
                        {
                            var par = ctor.GetParameters().FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                            if (par == null)
                            {
                                continue;
                            }
                            var def = GetDefault(par.ParameterType);
                            if (c.ContainsKey(name))
                            {
                                throw new Exception($"type {t} has {name} twice");
                            }
                            c.Add(name, def);
                            break;

                        }
                        if (c.Count != lenArgs)
                            continue;
                        else
                            break;

                    }
                }
                return null;
            }
            object GetDefault<T>(T type)
                where T : Type
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                    //return default(T); ;
                }
                if (type == typeof(string))
                {
                    return "";
                }
                return null;
            }
        }
    }
}
