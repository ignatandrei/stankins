import { Component, OnInit } from '@angular/core';

import { ConfigService } from '../config.service';
import { config } from 'rxjs';

@Component({
  selector: 'app-whatsimple',
  templateUrl: './whatsimple.component.html',
  styleUrls: ['./whatsimple.component.css']
})
export class WhatsimpleComponent implements OnInit {

  constructor(private cfg:ConfigService) { }

  ngOnInit() {
    this.cfg.GetStankinsAll().subscribe(it=>{
      window.alert(it.length);
    })
  }

}

