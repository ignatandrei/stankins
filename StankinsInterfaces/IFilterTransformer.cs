using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StankinsInterfaces
{
    public interface IFilterTransformer
    {
        IRow[] valuesRead { set; }
        IRow[] valuesTransformed { get; }

        Task Run();
    }
}
