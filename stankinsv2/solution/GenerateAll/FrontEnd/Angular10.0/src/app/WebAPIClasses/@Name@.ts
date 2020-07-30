@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.FindAfterName("@Name@").Value;
    var nrCols =dt.Columns.Count;
}


export class @dt.TableName
    {
        
        
        public constructor(other:@dt.TableName = null){ 

            if(other != null){
				this.CopyPropertiesFrom(other, true);
			}
                
        }
        public CopyPropertiesFrom(other:@dt.TableName, withID: boolean):void{
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

            
        }
        public  ID: number;
            
        @for(int iCol = 0;iCol < nrCols; iCol++){
            var col = dt.Columns[iCol];
            var colName= col.ColumnName ;
            var colType = col.DataType;
			var nameType ="";
			switch(colType.Name.ToLower()){
				case "string":
					nameType="string";
					break;
				default:
					nameType="!!!!"+@colType.Name;
					break;
			}

            <text>
            public @colName : @nameType;
            </text>

        }
        
    }

