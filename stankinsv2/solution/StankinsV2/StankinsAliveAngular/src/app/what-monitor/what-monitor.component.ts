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
        [this.columnsToDisplay, this.dataSource ] =  Configuration.TransformToRow(it.ExecutorsDynamic);
      }
    );
  }

  public friendlyName(fullName: string): string {
    return Configuration.friendlyName(fullName);
  }
}
