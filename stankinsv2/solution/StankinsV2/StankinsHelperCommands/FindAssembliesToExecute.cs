using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StankinsHelperCommands
{
    public class FindAssembliesToExecute
    {
        private readonly Assembly a;

        public FindAssembliesToExecute(Assembly a)
        {
            this.a = a;
        }
        public KeyValuePair<Type, CtorDictionary>[] FindAssemblies()
        {
            var ret = new List<KeyValuePair<Type, CtorDictionary>>();
            var types = a.GetExportedTypes();
            
            foreach (Type type in types)
            {
                var ctor = TryToConstruct(type);
                if(ctor == null)
                {
                    continue;
                }
                ret.Add(new KeyValuePair<Type, CtorDictionary>( type, ctor));

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

            var interf = typeof(IBaseObject);
            if (!interf.IsAssignableFrom(t))
            {
                return false;
            }
            return true;
        }
        CtorDictionary TryToConstruct(Type t)
        {
            if (!CanConstruct(t))
            {
                return null;
            }
            var c = new CtorDictionary();
            while (true)
            {
                try
                {
                    var res = Activator.CreateInstance(t, c);
                    if(c.Count() == 0)
                    {
                        //we need to add the arguments of a ctor 
                        var ctors = t.GetConstructors();
                        if (ctors.Any(it => it.GetParameters().Length == 0)){
                            return c;
                        }

                        foreach (var ctor in ctors)
                        {
                            if (!ctor.IsPublic)
                                continue;

                            var pars = ctor.GetParameters();
                            if (pars[0].ParameterType == typeof(CtorDictionary))
                                continue;
                            //first constructor ok
                            foreach(var parm in pars)
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
