using Stankins.Alive;
using Stankins.Amazon;
using Stankins.AnalyzeSolution;
using Stankins.AzureDevOps;
using Stankins.Cachet;
using Stankins.FileOps;
using Stankins.HTML;
using Stankins.Interfaces;
using Stankins.Office;
using Stankins.Process;
using Stankins.Razor;
using Stankins.Rest;
using Stankins.SimpleRecipes;
using Stankins.SqlServer;
using Stankins.Trello;
using Stankins.Version;
using Stankins.XML;
using StankinsCommon;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StankinsHelperCommands
{
    public class FindAssembliesToExecute
    {


        private static ResultTypeStankins[] refs;
        private static readonly object lockObj = new object();
        public static ResultTypeStankins[] AddReferences(ResultTypeStankins moreResult)
        {
            if (refs != null)
            {
                return refs;
            }

            lock (lockObj)
            {
                if (refs != null)
                {
                    return refs;
                }

                var allTypes = new List<ResultTypeStankins>();
                if (moreResult != null)
                    allTypes.Add(moreResult);
                
                FindAssembliesToExecute f = null;
                

                //throw new ArgumentException("cannot find ReceiveTableDatabaseSql" );
                f = new FindAssembliesToExecute(typeof(SenderCachet).Assembly);
                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(AliveStatus).Assembly);
                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ReceiverFromTrello).Assembly);
                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ReceiveTableDatabaseSql).Assembly);
                allTypes.AddRange(f.FindTypes());


                f = new FindAssembliesToExecute(typeof(FilterRemoveColumn).Assembly);
                allTypes.AddRange(f.FindTypes());


                f = new FindAssembliesToExecute(typeof(Stankins.Rest.ReceiveRest).Assembly);
                allTypes.AddRange(f.FindTypes());


                f = new FindAssembliesToExecute(typeof(AmazonMeta).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ReceiverFromSolution).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(YamlReader).Assembly);

                allTypes.AddRange(f.FindTypes());



                f = new FindAssembliesToExecute(typeof(ReceiverCSV).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ReceiverHtml).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(SenderExcel).Assembly);

                allTypes.AddRange(f.FindTypes());


                f = new FindAssembliesToExecute(typeof(ReceiverProcess).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(SenderDBDiagramToDot).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ExportDBDiagramHtmlAndDot).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(FileVersionFromDir).Assembly);

                allTypes.AddRange(f.FindTypes());

                f = new FindAssembliesToExecute(typeof(ReceiverXML).Assembly);

                allTypes.AddRange(f.FindTypes());

                refs = allTypes.ToArray();


                return refs;
            }
        }

        private readonly Assembly a;

        public FindAssembliesToExecute(Assembly a)
        { 
            this.a = a;
        }
        public ResultTypeStankins FromType(Type type)
        {
            CtorDictionary ctor = TryToConstruct(type);
            if (ctor == null)
            {
                return null;
            }



            return (new ResultTypeStankins(type, ctor));

        }
        public ResultTypeStankins[] FindTypes()
        {
            List<ResultTypeStankins> ret = new List<ResultTypeStankins>();
            Type[] types = a.GetExportedTypes();

            foreach (Type type in types)
            {
                var f = FromType(type);
                if (f == null)
                    continue;

                ret.Add(f);

            }
            return ret.ToArray();

        }
        private bool CanConstruct(Type t)
        {
            if (!t.FullName.Contains("tankins"))
            {
                return false;
            }
            if (!t.IsClass || t.IsAbstract)
            {
                return false;
            }
            if (t.FullName.Contains("BaseObjectInSerial"))
            {
                return false;
            }
            if (t.ContainsGenericParameters)
            {
                return false;
            }

            Type interf = typeof(IBaseObject);
            if (!interf.IsAssignableFrom(t))
            {
                return false;
            }
            return true;
        }

        private CtorDictionary TryToConstruct(Type t)
        {
            if (!CanConstruct(t))
            {
                return null;
            }
            CtorDictionary c = new CtorDictionary();
            while (true)
            {
                try
                {
                    var c1 = new CtorDictionary(c);
                    object res = Activator.CreateInstance(t, c1);
                    if (c.Count() == 0)
                    {
                        //we need to add the arguments of a ctor 
                        ConstructorInfo[] ctors = t.GetConstructors();
                        if (ctors.Any(it => it.GetParameters().Length == 0))
                        {
                            return c;
                        }

                        foreach (ConstructorInfo ctor in ctors)
                        {
                            if (!ctor.IsPublic)
                            {
                                continue;
                            }

                            ParameterInfo[] pars = ctor.GetParameters();
                            if (pars[0].ParameterType == typeof(CtorDictionary))
                            {
                                continue;
                            }
                            //first constructor ok
                            foreach (ParameterInfo parm in pars)
                            {
                                c.Add(parm.Name, GetDefault(parm.ParameterType));
                            }
                            break;
                        }
                    }
                    return c;
                }
                
                catch (TargetInvocationException tex)
                {
                    int lenArgs = c.Count;
                    string name =null;
                    var  innerArgEx = tex.InnerException as ArgumentException;
                    if (innerArgEx != null)
                    {
                        name=innerArgEx.ParamName;
                    }
                    if(innerArgEx == null)
                    {
                        var innerKeyEx= tex.InnerException as KeyNotFoundException;
                        if(innerKeyEx != null){
                            // The given key 'nameColumn' was not present in the dictionary.
                            name =innerKeyEx.Message;
                            var first=name.IndexOf("'");
                            var last= name.IndexOf("'",first+1);
                            name= name.Substring(first+1,last-first-1);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception($"for {t.Name} tex.InnerException is {tex.InnerException} ");
                    
                    }
                    if (c.ContainsKey(name))
                    {
                        throw new Exception($"type {t} has {name} twice");
                    }
                    foreach (ConstructorInfo ctor in t.GetConstructors())
                    {
                        ParameterInfo par = ctor.GetParameters().FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                        if (par == null)
                        {
                            continue;
                        }
                        object def = GetDefault(par.ParameterType);
                        if (c.ContainsKey(name))
                        {
                            throw new Exception($"type {t} has {name} twice");
                        }
                        c.Add(name, def);
                        break;

                    }
                    if (c.Count != lenArgs)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //trying to find with the constructor - not ctor dictionary
            try
            {

                object res = Activator.CreateInstance(t, c.Values.Select(it => it).ToArray());
                return c;
            }
            catch (Exception)
            {
                //do nothing - not successfull
                //trying to construct with first constructor that matches the values
                foreach (ConstructorInfo ctor in t.GetConstructors())
                {
                    List<ParameterInfo> pars = ctor.GetParameters().ToList();
                    int lenght = pars.Count;
                    pars.RemoveAll(it => c.ContainsKey(it.Name));
                    if (pars.Count == lenght)//not found arguments matching
                    {
                        continue;
                    }
                    CtorDictionary c1 = new CtorDictionary(c);
                    foreach (ParameterInfo par in pars)
                    {
                        object def = GetDefault(par.ParameterType);
                        c1[par.Name] = def;
                    }
                    try
                    {

                        object res = Activator.CreateInstance(t, c1.Values.Select(it => it).ToArray());
                        return c1;
                    }
                    catch (Exception)
                    {
                        //DO NOTHING-CTOR NOT GOOD
                    }

                }


            }
            return null;
        }

        private object GetDefault<T>(T type)
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
