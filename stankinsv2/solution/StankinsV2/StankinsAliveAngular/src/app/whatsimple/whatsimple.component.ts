import { Component, OnInit } from '@angular/core';

import { ConfigService } from '../config.service';
import { config } from 'rxjs';
import { ResultTypeStankins, WhatToList } from './ResultTypeStankins';

@Component({
  selector: 'app-whatsimple',
  templateUrl: './whatsimple.component.html',
  styleUrls: ['./whatsimple.component.css']
})
export class WhatsimpleComponent implements OnInit {

  constructor(private cfg:ConfigService) { }
  rs: ResultTypeStankins[] ;
  rsRecipes : ResultTypeStankins[] ;
  ngOnInit() {
    this.cfg.GetStankinsAll().subscribe(it => {
      this.rs = it;

      this.rsRecipes = it.filter((a)=>
        (WhatToList.Receivers === (a.cacheWhatToList & WhatToList.Receivers)) &&
        (WhatToList.Senders === (a.cacheWhatToList & WhatToList.Senders))
      );

    });
  }

}

