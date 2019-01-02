import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { element } from '@angular/core/src/render3';
import { VersionsNetcoreAngularService } from './versions-netcore-angular.service';
import { DisplayNetCoreComponent } from './display-net-core/display-net-core.component';
import { FileVersionInfo, PackageJSONVersion, Dependencies, Dependency, FVSAng } from './FileVersion';


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
  public dataSourceNET: MatTableDataSource<FileVersionInfo>;

  public dataSourceNPM: MatTableDataSource<FVSAng>;

  displayedColumnsNET: string[] = ['internalName', 'companyName', 'fileVersion', 'AllInfo'];

  @ViewChild(MatSort)
  sortNet: MatSort;

  displayedColumnsNPM: string[] = ['name', 'version', 'AllInfo'];

  @ViewChild(MatSort)
  sortAng: MatSort;
  ngOnInit() {
    console.log('ng on init versions');
    this.vs.FVSAngular().subscribe(it => {
      this.dataSourceNPM = new MatTableDataSource( it);
      this.dataSourceNPM.sort = this.sortAng;
    }
       );
    this.vs.FVS().subscribe(it => {
      console.log(it.length);
      this.dataSourceNET = new MatTableDataSource( it);
      this.dataSourceNET.sort = this.sortNet;
    });
  }
  applyFilterNET(filterValue: string) {
    this.dataSourceNET.filter = filterValue.trim().toLowerCase();
  }
  applyFilterNPM(filterValue: string) {
    this.dataSourceNPM.filter = filterValue.trim().toLowerCase();
  }
  infoElementNPM( el: FileVersionInfo) {
    const dialogRef = this.dialog.open(DisplayNetCoreComponent, {
      // width: '250px',
      data: el
    });
  }
  infoElementNET( el: FileVersionInfo) {
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
