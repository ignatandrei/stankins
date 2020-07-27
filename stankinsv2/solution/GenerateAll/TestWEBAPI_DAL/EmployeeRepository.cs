@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
    string repo= @dt.TableName  + "_BLRepository";
}
using System.Collections.Generic;

namespace TestWebAPI_BL
{
    public partial class @repo
    {
        public List<@dt.TableName> GetAll_@dt.TableName ()
        {
            return null;
        }
    }
}
