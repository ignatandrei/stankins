using System;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface IJob       
    {
        OrderedList<IReceive> Receivers { get; }
        OrderedList<IFilterTransformer> FiltersAndTransformers { get;  }        

        OrderedList<ISend> Senders { get;  }

        Task Execute();
    }
}
