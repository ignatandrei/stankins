using StankinsInterfaces;
using StanskinsImplementation;
using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ReceiverDll
{
    public class ReceiverFromDll : IReceive
    {

        public IRowReceive[] valuesRead { get; set; }
        public string Name { get; set; }
        public string DllFileName { get; set; }
        public async Task LoadData()
        {
            var d = AssemblyLoadContext.Default;
            var data = d.LoadFromAssemblyPath(DllFileName);
            var an = data.GetName();
            foreach (var item in data.GetExportedTypes())
            {
                var rr = new RowRead();
                rr.Values.Add("Name", item.Name);
                rr.Values.Add("FullName", item.FullName);
                rr.Values.Add("AssemblyName", an.Name);
                rr.Values.Add("AssemblyFullName", an.FullName);
            }
        }
    }
}
