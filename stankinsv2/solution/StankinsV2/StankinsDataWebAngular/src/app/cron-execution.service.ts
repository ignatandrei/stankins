import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export class CronExecutionFile {
  public name: string ;
  public cron: string;
  public content: string;

}
@Injectable({
  providedIn: 'root'
})
export class CronExecutionService {
  Save(cronexec: CronExecutionFile): Observable<object> {
    let url = environment.url;
    url += '/api/v1.0/CronExecutionFile';
    return this.http.put(url, cronexec);

  }

  constructor(private http: HttpClient) { }

  public Get(): Observable<CronExecutionFile[]> {
    let url = environment.url;
    url += '/api/v1.0/CronExecutionFile';
    return this.http.get<CronExecutionFile[]>(url);

  }
}
