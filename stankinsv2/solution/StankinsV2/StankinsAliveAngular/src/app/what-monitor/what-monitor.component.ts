import { Component, OnInit } from '@angular/core';
import { RowConfiguration, ConfigService, Configuration, Executor } from '../config.service';

@Component({
  selector: 'app-what-monitor',
  templateUrl: './what-monitor.component.html',
  styleUrls: ['./what-monitor.component.css']
})
export class WhatMonitorComponent implements OnInit {

  dataSource: RowConfiguration[];
  columnsToDisplay = [];
  constructor(private fg: ConfigService) { }

  ngOnInit() {
    this.fg.GetConfig('default').subscribe(
      it => {
        // window.alert(it.UserName);
        // window.alert(JSON.stringify(it.ExecutorsDynamic));
        this.dataSource = this.TransformToRow(it.ExecutorsDynamic);
      }
    );
  }
  public TransformToRow(execs: Array<Executor>): RowConfiguration[] {

    const types = execs.map(it => it.Data.Type).filter((value, index, self) => {
      return self.indexOf(value) === index;
    });
    this.columnsToDisplay = types;
    const ret: RowConfiguration[] = [];
    // window.alert(types);
    execs.forEach(it => {

        let foundType = false;
        const type = it.Data.Type;
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
    // const r = new RowConfiguration(types);
    // ret.push(r);
    // window.alert(ret.length );
    // ret.forEach( r => {
    //   window.alert(JSON.stringify(r));
    // });
    return ret;

  }
  public friendlyName(fullName: string): string {
    return Configuration.friendlyName(fullName);
  }
}
