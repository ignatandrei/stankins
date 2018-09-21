using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stankins.Interfaces
{
    public interface IStreaming<T>        
    {
        Task<bool> Initialize();
        IEnumerable<T> StreamTo(IDataToSent dataToSent);
    }
    public interface IStreamingReceive<T>
    {
        Task<bool> Initialize();
        Task<T[]> StreamData();

    }
}
