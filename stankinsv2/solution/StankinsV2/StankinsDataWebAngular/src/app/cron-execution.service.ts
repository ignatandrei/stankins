import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export class CronExecutionFile {

  public name: string ;
  public cron: string;
  public content: string;

  public static NewCron(): CronExecutionFile {
    const c = new CronExecutionFile();
    c.cron = '*/30 * * * * *';
    c.content = 'Stankins.Alive.ReceiverPing nameSite=www.yahoo.com';
    c.name = CronExecutionFile.newGuid();

    return c;
  }

  static newGuid(): string {
    let result: string;
    let i: string;
    let j: number;

    result = '';
    for (j = 0; j < 32; j++) {
      if (j === 8 || j === 12 || j === 16 || j === 20) {
        result = result + '';
      }
      i = Math.floor(Math.random() * 16)
        .toString(16)
        .toUpperCase();
      result = result + i;
    }
    return result;
  }


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
  public Delete(cronexec: CronExecutionFile): Observable<object>{
    let url = environment.url;
    url += '/api/v1.0/CronExecutionFile';
    url += '/'+ cronexec.name;
    return this.http.delete(url);
  }
  addNew(cronexec: CronExecutionFile): Observable<object> {
    let url = environment.url;
    url += '/api/v1.0/CronExecutionFile';
    return this.http.post(url, cronexec);

  }
  constructor(private http: HttpClient) { }

  public Get(): Observable<CronExecutionFile[]> {
    let url = environment.url;
    url += '/api/v1.0/CronExecutionFile';
    return this.http.get<CronExecutionFile[]>(url);

  }
}
