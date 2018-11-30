import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort } from '@angular/material';
import { MonitorTableDataSource } from './monitor-table-datasource';

@Component({
  selector: 'app-monitor-table',
  templateUrl: './monitor-table.component.html',
  styleUrls: ['./monitor-table.component.css'],
})
export class MonitorTableComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: MonitorTableDataSource;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['id', 'name'];

  ngOnInit() {
    this.dataSource = new MonitorTableDataSource(this.paginator, this.sort);
  }
}
