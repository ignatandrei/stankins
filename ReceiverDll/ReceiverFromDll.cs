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
    public abstract class ReceiverFromDll : IReceive
    {
        public IRowReceive[] valuesRead { get; set; }
        public string Name { get; set; }
        public string DllFileName { get; set; }
        public ReceiverFromDll(string dllFileName)
        {
            DllFileName = dllFileName;
        }
        abstract public void ProcessTypeInfo(TypeInfo item, Assembly assembly);
        abstract public void LoadingAssembly(Assembly assembly);
        abstract public void EndLoadingAssembly(Assembly assembly);
        public async Task LoadData()
        {
            
            var d = AssemblyLoadContext.Default;
            var data = d.LoadFromAssemblyPath(DllFileName);
            LoadingAssembly(data);
            foreach (var item in data.DefinedTypes)
            {
                ProcessTypeInfo(item,data);
            }
            EndLoadingAssembly(data);
            await Task.CompletedTask;
        }
        public void ClearValues()
        {
            valuesRead = null;
        }
    }
    public class ReceiverFromDllPlain : ReceiverFromDll
    {
        public ReceiverFromDllPlain(string dllFileName):base(dllFileName)
        {
            
        }
        
        public bool LoadInterfaces { get; set; }
        public bool LoadBaseClasses { get; set; }
        List<IRowReceive> ret;
        AssemblyName an;
        

        public override void LoadingAssembly(Assembly assembly)
        {
            ret = new List<IRowReceive>();
            an = assembly.GetName();
        }

        public override void ProcessTypeInfo(TypeInfo item, Assembly assembly)
        {
            string interfaces = "", baseTypes = "";
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

        public override void EndLoadingAssembly(Assembly assembly)
        {
            valuesRead = ret.ToArray();
        }
    }
}
