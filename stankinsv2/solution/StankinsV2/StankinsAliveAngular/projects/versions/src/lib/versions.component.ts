import { Component, OnInit } from '@angular/core';
import { VersionsService } from './versions.service';
import { FileVersionInfo } from './FileVersion';
import { MatTableDataSource } from '@angular/material';
import { element } from '@angular/core/src/render3';


@Component({
  selector: 'vers-versions',
  templateUrl: './versions.component.html',
  styles: []
})
export class VersionsComponent implements OnInit {

  constructor(private vs: VersionsService) { }
  public fvs: FileVersionInfo[];
  public dataSource: MatTableDataSource<FileVersionInfo>;
  displayedColumns: string[] = ['internalName', 'companyName', 'fileVersion', 'AllInfo'];
  ngOnInit() {
    console.log('ng on init versions');
    this.vs.FVS().subscribe(it => {
      console.log(it.length);
      this.dataSource = new MatTableDataSource( it);

    });
  }
  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  infoElement( el: FileVersionInfo): string {

    let ret = 'info';

    for (const key in el) {
      if (!el.hasOwnProperty(key)) {
        continue;
      }
      const val = el[key];
      if (('' + val ).trim().length > 0) {
        ret += `${key}=${val};`;
      }
    }
    return ret;
  }
}
