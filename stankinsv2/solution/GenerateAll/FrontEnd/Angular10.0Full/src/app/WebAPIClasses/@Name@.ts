@model Stankins.Interfaces.IDataToSent
@{

    var dt= Model.FindAfterName("@Name@").Value;
    var nrCols =dt.Columns.Count;
	string lowerCaseFirst(string s){
		return char.ToLower(s[0]) + s.Substring(1);
	}
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
                this.id= other.id;
            }
            @for(int iCol = 0;iCol < nrCols; iCol++){
                var col = dt.Columns[iCol];
                var colName= col.ColumnName ;
                
                <text>
            this.@lowerCaseFirst(colName) = other.@lowerCaseFirst(colName);
                </text>

            }

            
        }
        public  id: number;
            
        @for(int iCol = 0;iCol < nrCols; iCol++){
            var col = dt.Columns[iCol];
            var colName= col.ColumnName ;
            var colType = col.DataType;
			var nameType ="";
			switch(colType.Name.ToLower()){
				case "string":
					nameType="string";
					break;
				case "decimal":
					nameType="number";
					break;
				default:
					nameType="!!!!"+@colType.Name;
					break;
			}

            <text>
            public @lowerCaseFirst(colName) : @nameType;
            </text>

        }
        
    }

