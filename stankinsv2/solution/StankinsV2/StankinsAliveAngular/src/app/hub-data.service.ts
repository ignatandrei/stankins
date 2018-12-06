import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, ReplaySubject } from 'rxjs';
import { ResultWithData } from './DTO/HubDeclaration';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class HubDataService {
  private connection: signalR.HubConnection;
  private subject:  Subject<ResultWithData>;

  constructor(private http: HttpClient) {

    this.subject = new ReplaySubject<ResultWithData>(20);
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/DataHub')
      .build();


   this.connection
    .start()
    .catch(err => document.write(err) )
    .finally(() => {
      http.get('api/values').subscribe();
    });
    const self = this;

    this.connection.on('sendMessageToClients', o => {
      const p = o as ResultWithData;
      p.aliveResult.startedDate = new Date(p.aliveResult.startedDate.toString());
      // time zone offset
      const dif = new Date().getTimezoneOffset();
      p.aliveResult.startedDate = new Date( p.aliveResult.startedDate.valueOf() - dif * 60000);
      self.subject.next(p);
    });


  }
  public getData(): Observable <ResultWithData> {
    return this.subject.asObservable();
  }
}
