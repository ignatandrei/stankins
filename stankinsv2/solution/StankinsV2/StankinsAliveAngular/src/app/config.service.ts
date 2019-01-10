import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { switchMap, tap, map } from 'rxjs/operators';
const httpOptionsPlain = {
  headers: new HttpHeaders({
    'Content-Type':  'text/plain'
  })
};
@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  constructor(private http: HttpClient) {

  }
  public GetConfig(id: string): Observable<Configuration> {

      const url = '/api/values/' + id;
      // window.alert(url);
      return this.http.get<Configuration>(url, httpOptionsPlain);

      // return of(new Configuration());
  }
}

export class RowConfiguration {

  constructor(props: string[]) {
    const self = this;
    props.forEach(it => self[it] = null);
  }
  public friendlyName(typeAlive: string) {
    return typeAlive;
  }


}
export class Configuration {
  public UserName: string;
  public ExecutorsDynamic: Executor[];
}
export class Executor {
  public Data: Data;
  public CustomData: CustomData;
}
export  class Data {
  Type: string;
  CRON: string;
}
export class CustomData {
  Name: string;
  Tags: string;
  Icon: string;
}
