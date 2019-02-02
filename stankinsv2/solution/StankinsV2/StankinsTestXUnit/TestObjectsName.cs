using FluentAssertions;
using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    //[Trait("Verify", "")]
    public class TestObjectsName
    {
        static List<string> verifiedAssembly = new List<string>();
        //[Fact]
        public void VerifyNames()
        {
            VerifyName(Assembly.GetExecutingAssembly());
        }
        
        private void VerifyName(Assembly a)
        {
            if(verifiedAssembly.Contains(a.FullName))
            {
                return;
            }

            verifiedAssembly.Add(a.FullName);

            VerifyTypes(a);

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
                VerifyName(assembly);
            }
            return;
        }
        
        private void VerifyTypes(Assembly a)
        {
            if (!a.FullName.Contains("tankins"))
                return;

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
                if (new[] { "FilterColumnDataGreaterThanLength"
                    ,"FilterRetainColumnDataContains"
                    ,"ReceiverDBSqlServer"
                    ,"ReceiveDatabasesSql"
                    ,"ReceiveMetadataFromDatabaseSql"
                    ,"ReceiveTableDatabaseSql"
                    ,"ReceiveQueryFromDatabaseSql"
                    ,"DBReceiverStatement"
                }.Contains(t.Name))
                {
                    continue;
                }
                var obj = TryToConstruct(t) as IBaseObject;
                Assert.True(obj != null, $"type {t.Name} should be constructed");

                //$"the type {t.Name} should be constructed".w(() => obj.Should().NotBeNull());
                obj.Name.Should().Be(t.Name, $"the type {t.Name} is constructed");
                //$"and the  {t.Name} should be {obj.Name}".w(() => obj.Name.Should().Be(t.Name));

            }
        }

        private IBaseObject TryToConstruct(Type t)
        {
            var c = new CtorDictionary();
            while (true)
            {
                try
                {
                    var res = Activator.CreateInstance(t, c);
                    return res as IBaseObject;
                }
                catch(TargetInvocationException tex)
                {
                    var lenArgs = c.Count;
                    var ex = tex.InnerException as ArgumentException;
                    if(ex == null)
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
                        var par = ctor.GetParameters().FirstOrDefault(p => p.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase));
                        if(par == null)
                        {
                            continue;
                        }
                        var def = GetDefault(par.ParameterType);
                        if (c.ContainsKey(name))
                        {
                            throw new Exception($"type {t} has {name} twice");
                        }
                        c.Add(name,def );
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
        public static object GetDefault<T>(T type)
            where T:Type
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
                //return default(T); ;
            }
            if (type == typeof(string))
            {
                return "" ;
            }
            return null;
        }
    }
}
