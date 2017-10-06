using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ReceiverDll
{
    public class ReceiverFromDll : IReceive
    {
        public ReceiverFromDll(string dllFileName)
        {
            DllFileName = dllFileName;
        }
        public IRowReceive[] valuesRead { get; set; }
        public string Name { get; set; }
        public string DllFileName { get; set; }
        public bool LoadInterfaces { get; set; }
        public bool LoadBaseClasses { get; set; }
        public async Task LoadData()
        {
            var ret = new List<IRowReceive>();
            var d = AssemblyLoadContext.Default;
            var data = d.LoadFromAssemblyPath(DllFileName);
            var an = data.GetName();
            foreach (var item in data.DefinedTypes)
            {
                string interfaces="", baseTypes="";
                if (LoadInterfaces)
                {
                    var i = item.ImplementedInterfaces.ToArray();
                    interfaces = string.Join("@", i.Select(it => it.FullName).ToArray());
                }
                if (LoadBaseClasses)
                {
                    var bases = new List<string>();
                    var baseType = item.BaseType;
                    while (baseType != typeof(object))
                    {
                        bases.Add(baseType.FullName);
                        baseType = baseType.BaseType;
                    }
                    bases.Add(baseType.FullName);//object

                    baseTypes = string.Join("@", bases.ToArray());
                }
                var rr = new RowRead();
                rr.Values.Add("Name", item.Name);
                rr.Values.Add("FullName", item.FullName);
                rr.Values.Add("AssemblyName", an.Name);
                if (LoadInterfaces)
                {
                    rr.Values.Add("Interfaces", interfaces);
                }
                if (LoadBaseClasses)
                {
                    rr.Values.Add("BaseTypes", baseTypes);
                }
                rr.Values.Add("AssemblyFullName", an.FullName);
                
                ret.Add(rr);
            }
            valuesRead = ret.ToArray();
        }
    }
}
