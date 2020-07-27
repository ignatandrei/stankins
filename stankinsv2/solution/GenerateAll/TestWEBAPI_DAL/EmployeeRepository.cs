@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
    string repo= @dt.TableName  + "_Repository";
}
using System.Collections.Generic;

namespace TestWebAPI_BL
{
    public partial class @repo
    {
        private readonly DatabaseContext databaseContext;

        public @repo (DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public List<@dt.TableName> GetAll_@dt.TableName ()
        {
            return null;
        }
    }
}
