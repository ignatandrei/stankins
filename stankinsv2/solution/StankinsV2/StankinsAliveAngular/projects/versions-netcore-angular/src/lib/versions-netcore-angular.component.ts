import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { element } from '@angular/core/src/render3';
import { VersionsNetcoreAngularService } from './versions-netcore-angular.service';
import { DisplayNetCoreComponent } from './display-net-core/display-net-core.component';
import { FileVersionInfo } from './FileVersion';


@Component({
  selector: 'vers-versions-netcore-angular',
  templateUrl: './versions-netcore-angular.component.html',
  styles: []
})
export class VersionsNetcoreAngularComponent implements OnInit {

  constructor(private vs: VersionsNetcoreAngularService,
    public dialog: MatDialog
    ) { }
  public fvs: FileVersionInfo[];
  public dataSource: MatTableDataSource<FileVersionInfo>;
  displayedColumns: string[] = ['internalName', 'companyName', 'fileVersion', 'AllInfo'];
  @ViewChild(MatSort)
  sort: MatSort;


  ngOnInit() {
    console.log('ng on init versions');
    this.vs.FVSAngular().subscribe(it => window.alert(JSON.stringify(it)));
    this.vs.FVS().subscribe(it => {
      console.log(it.length);
      this.dataSource = new MatTableDataSource( it);
      this.dataSource.sort = this.sort;
    });
  }
  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  infoElement( el: FileVersionInfo) {
    const dialogRef = this.dialog.open(DisplayNetCoreComponent, {
      // width: '250px',
      data: el
    });
    // let ret = 'info';

    // for (const key in el) {
    //   if (!el.hasOwnProperty(key)) {
    //     continue;
    //   }
    //   const val = el[key];
    //   if (('' + val ).trim().length > 0) {
    //     ret += `${key}=${val};`;
    //   }
    // }
    // return ret;
  }
}
