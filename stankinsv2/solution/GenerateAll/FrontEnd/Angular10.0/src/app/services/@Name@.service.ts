@model Stankins.Interfaces.IDataToSent
@{
	var angular="@angular";
	var Injectable = "@Injectable";
		var dt= Model.FindAfterName("@Name@").Value;
		var nameTable =dt.TableName;
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

  baseUrl: string;
  constructor(private client: HttpClient) { 
    this.baseUrl = environment.webAPIUrl;

  }

  public GetAll() : Observable<@(nameTable)[]>{
    const url = this.baseUrl+'api/@nameTable/GetAll';
    
    return this.client.get<@(nameTable)[]>(url);
  }

  
}
