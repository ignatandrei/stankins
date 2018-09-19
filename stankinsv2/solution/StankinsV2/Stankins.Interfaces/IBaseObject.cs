using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stankins.Interfaces
{
    public interface IBaseObject
    {
        string Name { get; set; }
        IDictionary<string,object> StoringDataBetweenCalls{ get; set; }
        Version Version { get; }
        Task<IDataToSent> TransformData(IDataToSent receiveData);
        Task<IMetadata> TryLoadMetadata();
    }
}
