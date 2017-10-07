using StankinsInterfaces;
using StanskinsImplementation;
using System.Collections.Generic;
using System.Reflection;

namespace ReceiverDll
{
    public class ReceiverFromDllRelational : ReceiverFromDll
    {
        RowReadRelation rr;
        public ReceiverFromDllRelational(string dllFileName) : base(dllFileName)
        {
                
        }
        public override void EndLoadingAssembly(Assembly assembly)
        {
            valuesRead = new IRowReceive[1] { rr };
        }

        public override void LoadingAssembly(Assembly assembly)
        {
            rr = new RowReadRelation();
            AssemblyName an = assembly.GetName();
            rr.Values.Add("AssemblyName", an.FullName);
            rr.Values.Add("Name", an.Name);
            var types = new List<IRowReceiveRelation>();
            rr.Relations.Add("Types", types);
        }

        public override void ProcessTypeInfo(TypeInfo item, Assembly assembly)
        {

            var types = rr.Relations["Types"];
            var typeRR = new RowReadRelation();
            typeRR.Values.Add("Name", item.Name);
            typeRR.Values.Add("FullName", item.FullName);
            
            types.Add(typeRR);
            var interfaces = new List<IRowReceiveRelation>();
            typeRR.Relations.Add("Interfaces", interfaces);
            foreach (var @interface in item.ImplementedInterfaces)
            {
                var intRR = new RowReadRelation();
                intRR.Values.Add("Name", @interface.Name);
                intRR.Values.Add("FullName", @interface.FullName);
                intRR.Values.Add("IsGeneric", @interface.IsGenericType);
                intRR.Values.Add("IsAbstract", @interface.IsAbstract);
                interfaces.Add(intRR);

            }
            var props = new List<IRowReceiveRelation>();
            typeRR.Relations.Add("Properties", props);
            foreach (var prop in item.GetProperties(BindingFlags.Public|BindingFlags.Instance))
            {
                var propRR = new RowReadRelation();
                propRR.Values.Add("Name", prop.Name);
                props.Add(propRR);
            }


        }
    }
}
