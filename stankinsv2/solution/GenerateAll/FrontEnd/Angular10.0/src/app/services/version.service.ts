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

  baseUrl: string;
  constructor(private client: HttpClient) { 
    this.baseUrl = environment.webAPIUrl;

  }

  public VersionGenerator() : Observable<string>{
    const url = this.baseUrl+'api/version/VersionGenerator';
    
    return this.client.get(url,{
      responseType: 'text' as const,
    });
  }

  public VersionBackend() : Observable<string>{
    const url = this.baseUrl+'api/version/VersionBackend';
    
    return this.client.get(url,{
      responseType: 'text' as const,
    });
  }



}
