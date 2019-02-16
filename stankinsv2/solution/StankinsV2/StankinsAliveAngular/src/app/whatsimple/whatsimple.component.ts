import { Component, OnInit } from '@angular/core';

import { ConfigService } from '../config.service';
import { config } from 'rxjs';
import { ResultTypeStankins, WhatToList } from './ResultTypeStankins';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-whatsimple',
  templateUrl: './whatsimple.component.html',
  styleUrls: ['./whatsimple.component.css']
})
export class WhatsimpleComponent implements OnInit {

  constructor(private cfg: ConfigService) { }
  // rs: ResultTypeStankins[] ;
  rsRecipes: ResultTypeStankins[] ;
  
  rsAll = new  Map<WhatToList, ResultTypeStankins[]>();
  ngOnInit() {
    this.cfg.GetStankinsAll().subscribe(it => {
      // this.rs = it;

      for (const value in WhatToList) {
        if (typeof WhatToList[value] === 'number') {
            console.log(WhatToList[value]);
            const w: WhatToList = +WhatToList[value];
            this.rsAll = this.rsAll.set(w, this.filterArr(it, w));
        }

        // const w: WhatToList  = what;
        // this.rsAll.set(w, this.filterArr(it, w));
     }
     
     const setReceivers = new Set(this.rsAll.get(WhatToList.Receivers));
     const setSenders = new Set(this.rsAll.get(WhatToList.Senders));
     this.rsRecipes = Array.from(setReceivers).filter(x => setSenders.has(x));

    });
  }
  private filterArr(arr: ResultTypeStankins[] , w: WhatToList ): ResultTypeStankins[] {

    const f = arr.filter(it => w === (it.cacheWhatToList & w));
    console.log(`enter ${w} number ${f.length}`);
    return f;
  }
}

