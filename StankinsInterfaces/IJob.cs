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
    public interface ISimpleJob    :IJob   
    {
        OrderedList<IReceive> Receivers { get; }
        OrderedList<IFilterTransformer> FiltersAndTransformers { get;  }        

        OrderedList<ISend> Senders { get;  }

        
    }

}
