import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FileVersionInfo } from '../FileVersion';

@Component({
  selector: 'vers-display-net-core',
  templateUrl: './display-net-core.component.html',
  styleUrls: ['./display-net-core.component.css']
})
export class DisplayNetCoreComponent implements OnInit {

  constructor( public dialogRef: MatDialogRef<FileVersionInfo>,
    @Inject(MAT_DIALOG_DATA) public data: FileVersionInfo) { }

  ngOnInit() {
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

}
