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
export class MonitorNavComponent implements OnInit {


  isHandset$: Observable<boolean> = this.breakpointObserver
  .observe(Breakpoints.Handset)
  .pipe(map(result => result.matches));



  public lastDateUpdated: Date;
  constructor( private breakpointObserver: BreakpointObserver,  private data: HubDataService) {


  }

  ngOnInit(): void {
    this.data.getData().subscribe(it => this.lastDateUpdated = it.aliveResult.startedDate);
  }

  // https://twitter.com/davidfowl/status/998043928291983360
  adapt<T>(st: signalR.IStreamResult<T>): Observable<T> {
    const subject = new Subject<T>();
    st.subscribe(subject);
    return subject.asObservable();
  }
}
