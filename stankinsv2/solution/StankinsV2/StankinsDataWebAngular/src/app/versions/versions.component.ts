import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-versions',
  templateUrl: './versions.component.html',
  styleUrls: ['./versions.component.css']
})
export class VersionsComponent implements OnInit {
  public swaggerUrl = '/swagger';
  constructor() { }

  ngOnInit() {

    this.swaggerUrl = environment.url + this.swaggerUrl ;
    this.swaggerUrl = `<a href=${this.swaggerUrl}>Swagger</a>`;
  }

}
