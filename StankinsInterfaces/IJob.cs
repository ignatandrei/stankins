using System;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface IJob
    {
        Task Execute();
        string SerializeMe();

        void UnSerialize(string serializeData);

    }
    public class RuntimeParameter
    {
        
        public string[] NameObjectsToApplyTo { get; set; }
        public string VariableName { get; set; }
    }
    public interface ISimpleJob    :IJob   
    {
        OrderedList<IReceive> Receivers { get; }
        OrderedList<IFilterTransformer> FiltersAndTransformers { get;  }        

        OrderedList<ISend> Senders { get;  }
        RuntimeParameter[] RuntimeParameters { get; }

    }

}
