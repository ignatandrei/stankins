import { Component, OnInit } from '@angular/core';
import { VersionsService } from './versions.service';
import { FileVersionInfo } from './FileVersion';


@Component({
  selector: 'vers-versions',
  template: `
    <p>
      Number of version files:{{fvs?.length}}
    </p>
    <div *ngFor='let item of fvs'>
    {{item.fileName}}  -  {{item.fileVersion}}
</div>

  `,
  styles: []
})
export class VersionsComponent implements OnInit {

  constructor(private vs: VersionsService) { }
  public fvs: FileVersionInfo[];

  ngOnInit() {
    this.vs.FVS().subscribe(it => {
      this.fvs = it;
    });
  }

}
