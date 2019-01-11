import { Component, OnInit } from '@angular/core';
import { HubDataService } from '../hub-data.service';
import { ResultWithData } from '../DTO/HubDeclaration';
import * as moment from 'moment';
import { RowConfiguration, Configuration } from '../config.service';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public results: Map<string, ResultWithData>;
public OK: ResultWithData[];
public Failed: ResultWithData[];
public test = 3065;
public OKRows: RowConfiguration[];
public OKColumns: string[];

public FailedRows: RowConfiguration[];
public FailedColumns: string[];


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

        [self.OKColumns, self.OKRows] = self.TransformToRow(self.OK);

        [self.FailedColumns, self.FailedRows] = self.TransformToRow(self.Failed);

    });
  }
  public friendlyName(fullName: string): string {
    return Configuration.friendlyName(fullName);
  }

  public  TransformToRow(execs: Array<ResultWithData>): [string[], RowConfiguration[]] {

    const types = execs.map(it => it.myType).filter((value, index, self) => {
      return self.indexOf(value) === index;
    });
    const ret: RowConfiguration[] = [];
    // window.alert(types);
    execs.forEach(it => {

        let foundType = false;
        const type = it.myType;
        ret.forEach( r => {
          const exist = (r[type] != null) ; // (JSON.stringify(r[type]).length > 0);
          if (!exist) {
            r[type] = it;
            foundType = true;
            return;
          }
        });
        if (!foundType) {
          const r = new RowConfiguration(types);
          r[type] = it;
          ret.push(r);
        }
    });
    return [types, ret];

    }

  constructor(private data: HubDataService) {

    this.results = new Map<string, ResultWithData>();

  }

}
