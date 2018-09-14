using System;
using System.Collections.Generic;
using System.Text;

namespace StankinsV2Interfaces
{
    public interface IStreaming<T>        
    {
        IEnumerable<T> StreamTo(IDataToSent dataToSent);
    }
}
