@model Stankins.Interfaces.IDataToSent
@{
	var angular="@angular";
	var Injectable = "@Injectable";
		var dt= Model.FindAfterName("@Name@").Value;
		var nameTable =dt.TableName;
		var nrRows =dt.Rows.Count; 
        var nrColumns = dt.Columns.Count;
                
}
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import{ @nameTable } from './../WebAPIClasses/@nameTable';

@(Injectable)({
  providedIn: 'root'
})
export class @(nameTable)Service {

  constructor() { 
    

  }

  public GetAll() : Observable<@(nameTable)[]>{
    var arrData:@(nameTable)[] =[];
	var newItem:@(nameTable)=null;
	
	@for(var iRow=0;iRow<nrRows;iRow++){
		string text="";
		for(var iCol=0;iCol<nrColumns;iCol++){
			text+=System.Environment.NewLine;
			var column=dt.Columns[iCol];
			string nameColumn = column.ColumnName;
			switch(column.DataType.Name.ToLower()){
				case "string":
					text+="newItem."+  nameColumn +" = " + "'" + dt.Rows[iRow][iCol] + "'" ;
					break;
				case "int32":
					text+="newItem."+  nameColumn +" = " +  dt.Rows[iRow][iCol]  ;
					break;
				case "decimal":
					text+="newItem."+  nameColumn +" = " +  dt.Rows[iRow][iCol]  ;
					break;
				default:
					text+="newItem."+ column.DataType.Name +"???"+ nameColumn +" = "+ dt.Rows[iRow][iCol];  
					break;  
			};
			text+=";";
			
		}
		<text>
		newItem = new @(dt.TableName)();
		newItem.id =@(iRow+1);
		
		@Raw(text) 
		arrData.push(newItem);
		
		</text>
	}
    return of(arrData);
  }

  
}
