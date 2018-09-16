using System.Collections.Generic;
using System.Data;

namespace StankinsV2Interfaces
{
    public interface IDataToSent
    {
        //TODO:move this in class DataToBeSentFurther
        int AddNewTable(DataTable dt);
        Dictionary<int,DataTable> DataToBeSentFurther { get; set; }
        IMetadata Metadata { get; set; }
        DataTable FindAfterName(string nameTable);

    }
}
