using System.Collections.Generic;
using System.Data;

namespace StankinsV2Interfaces
{
    public interface IDataToSent
    {
        Dictionary<int,DataTable> DataToBeSentFurther { get; set; }
        IMetadata Metadata { get; set; }
        DataTable FindAfterName(string nameTable);

    }
}
