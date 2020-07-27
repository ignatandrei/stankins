@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.DataToBeSentFurther[0];
}
using System;

namespace TestWebAPI_BL
{
    public partial class @dt.TableName
    {

        partial void OnConstructor();
        @{
            var nrCols =dt.Columns.Count;

        }
        #region constructors
        public @dt.TableName (){
            OnConstructor();
        }
        
        public @dt.TableName  (  @dt.TableName other):base(){ 
            @for(int iCol = 0;iCol < nrCols; iCol++){
                var col = dt.Columns[iCol];
                var colName= col.ColumnName ;
                
                <text>
            this.@colName = other.@colName;
                </text>

            }    
        }
        #endregion
        #region Properties
        @for(int iCol = 0;iCol < nrCols; iCol++){
            var col = dt.Columns[iCol];
            var colName= col.ColumnName ;
            var colType = col.DataType;

            <text>
            public @colType.Name @colName { get; set; }
            </text>

        }
        #endregion
        
    }
}
