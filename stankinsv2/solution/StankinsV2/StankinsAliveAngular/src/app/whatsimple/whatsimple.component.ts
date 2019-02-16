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

  WhatToList = WhatToList;
  constructor(private cfg: ConfigService) { }
  // rs: ResultTypeStankins[] ;
  rsRecipes: ResultTypeStankins[] ;
  allWhat: WhatToList[];
  recipeSelected: ResultTypeStankins;
  rsAll = new  Map<WhatToList, ResultTypeStankins[]>();
  ngOnInit() {
    this.cfg.GetStankinsAll().subscribe(it => {
      // this.rs = it;
      console.log(`TOTAL : ${it.len}`);
      for (const value in WhatToList) {
        if (typeof WhatToList[value] === 'number') {
            const w: WhatToList = +WhatToList[value];
            this.rsAll = this.rsAll.set(w, this.filterArr(it, w));
        }

        // const w: WhatToList  = what;
        // this.rsAll.set(w, this.filterArr(it, w));
     }

     this.allWhat = Array.from(this.rsAll.keys());

     const setReceivers = new Set(this.rsAll.get(WhatToList.Receivers));
     const setSenders = new Set(this.rsAll.get(WhatToList.Senders));
     console.log(`${setReceivers.size} ---- ${setSenders.size}`);
     this.rsRecipes = Array.from(setReceivers).filter(x => setSenders.has(x));

    });
  }
  private filterArr(arr: ResultTypeStankins[] , w: WhatToList ): ResultTypeStankins[] {
    // tslint:disable-next-line:no-bitwise
    let f = arr.filter(it => w === (w & it.cacheWhatToList) );
    if (w === WhatToList.None) {
      f = arr.filter(it => w ===  it.cacheWhatToList) ;
    }
    return f;
  }
}

