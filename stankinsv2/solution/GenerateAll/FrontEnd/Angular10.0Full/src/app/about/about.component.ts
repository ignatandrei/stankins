@{
  var angular="@angular";
  var Component="@Component";
}
import { Component, OnInit } from '@angular/core';
import {VersionService} from './../services/version.service';
@(Component)({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements OnInit {

  versionGenerator: string; 
  constructor(private version:VersionService) { }

  ngOnInit(): void {
    this.version.VersionGenerator().subscribe(it=>{
      this.versionGenerator=it;
    });
    
  }

}
