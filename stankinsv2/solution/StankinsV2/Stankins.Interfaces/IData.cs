using System.Collections.Generic;
using System.Data;

namespace Stankins.Interfaces
{
    public interface IDataToSent
    {
        string id{get;}
        //TODO:move this in class DataToBeSentFurther
        int AddNewTable(DataTable dt);
        Dictionary<int,DataTable> DataToBeSentFurther { get; set; }
        IMetadata Metadata { get; set; }
        KeyValuePair<int, DataTable> FindAfterName(string nameTable);

    }
}
