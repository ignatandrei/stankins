import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
interface KeyValuePair {
  key: string;
  value: string;
}
@Injectable({
  providedIn: 'root'
})
export class WritablesService {

  constructor(private http: HttpClient) { }

  public GetAll(): Observable<KeyValuePair[]> {
    let url = environment.url;
    url += '/api/v1.0/Writables';
    return this.http.get<KeyValuePair[]>(url);

  }
  public Save(kv: KeyValuePair) {
    let url = environment.url;
    url += '/api/v1.0/Writables';
    this.http.post(url, kv);
  }
  public Delete(id: string) {
    let url = environment.url;
    url += '/api/v1.0/Writables/' + id;
    this.http.delete(url);
  }



}
