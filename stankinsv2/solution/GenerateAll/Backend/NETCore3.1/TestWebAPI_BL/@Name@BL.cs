@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.FindAfterName("@Name@").Value;
    var nrCols =dt.Columns.Count;
}
using System;

namespace TestWebAPI_BL
{
    public partial class @dt.TableName
    {
        #region partial functions
        partial void OnConstructor();
        partial void OnCopyConstructor(@dt.TableName other, bool withID);
        #endregion

        #region constructors
        public @dt.TableName (){
            OnConstructor();
        }
        
        public @(dt.TableName)(@dt.TableName other):base(){ 

            OnCopyConstructor(other:other,withID: false);
                
        }
        public void CopyPropertiesFrom(@dt.TableName other, bool withID){
            if(withID){
                this.ID= other.ID;
            }
            @for(int iCol = 0;iCol < nrCols; iCol++){
                var col = dt.Columns[iCol];
                var colName= col.ColumnName ;
                
                <text>
            this.@colName = other.@colName;
                </text>

            }

            OnCopyConstructor(other,withID);
        }

        #endregion
        
        #region Properties
        public long ID{get;set;}
            
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
