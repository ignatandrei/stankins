import { Component, OnInit } from '@angular/core';
import { HubDataService } from '../hub-data.service';
import { ResultWithData } from '../DTO/HubDeclaration';
import * as moment from 'moment';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public results: Map<string, ResultWithData>;
public OK: ResultWithData[];
public Failed: ResultWithData[];

  ngOnInit(): void {
    const self = this;

      this.data.getData().subscribe(p => {
        // window.alert(JSON.stringify(p));
      // console.log('received ' + JSON.stringify(p));
      // console.log('received' + p.customData.name);
      self.results.set(p.customData.name, p);
      // p.cronExecution['textDate'] = moment(p.cronExecution.nextRunTime).from(moment());
      self.OK = Array.from(self.results.values())
        .filter((it: ResultWithData) => it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));

      self.Failed = Array.from(self.results.values())
        .filter((it: ResultWithData) => !it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));
    });
  }



  constructor(private data: HubDataService) {

    this.results = new Map<string, ResultWithData>();

  }

}
