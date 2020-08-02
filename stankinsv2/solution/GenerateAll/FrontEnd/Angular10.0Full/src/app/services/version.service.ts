@{
  var angular="@angular";
  var Injectable="@Injectable";

}
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@(Injectable)({
  providedIn: 'root'
})
export class VersionService {

  
  constructor() { 
    
  }

  public VersionGenerator() : Observable<string>{
    return of('1.2020');
  }
}
