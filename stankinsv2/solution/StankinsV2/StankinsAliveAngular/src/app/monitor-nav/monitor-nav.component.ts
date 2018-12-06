import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import * as signalR from '@aspnet/signalr';
import { ResultWithData } from '../DTO/HubDeclaration';
import { constructor } from 'q';
import { HubDataService } from '../hub-data.service';

@Component({
  selector: 'app-monitor-nav',
  templateUrl: './monitor-nav.component.html',
  styleUrls: ['./monitor-nav.component.css']
})
export class MonitorNavComponent implements  OnInit {


  isHandset$: Observable<boolean> = this.breakpointObserver
  .observe(Breakpoints.Handset)
  .pipe(map(result => result.matches));

public results: Map<string, ResultWithData>;
public OK: ResultWithData[];
public Failed: ResultWithData[];

  ngOnInit(): void {
    const self = this;

      this.data.getData().subscribe(p => {
        // window.alert(JSON.stringify(p));
      // console.log('received ' + JSON.stringify(p));
      console.log('received' + p.customData.name);
      self.results.set(p.customData.name, p);

      self.OK = Array.from(self.results.values())
        .filter((it: ResultWithData) => it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));

      self.Failed = Array.from(self.results.values())
        .filter((it: ResultWithData) => !it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));
    });
  }



  constructor(private data: HubDataService, private breakpointObserver: BreakpointObserver) {

    this.results = new Map<string, ResultWithData>();

  }
  // https://twitter.com/davidfowl/status/998043928291983360
  adapt<T>(st: signalR.IStreamResult<T>): Observable<T> {
    const subject = new Subject<T>();
    st.subscribe(subject);
    return subject.asObservable();
  }
}
