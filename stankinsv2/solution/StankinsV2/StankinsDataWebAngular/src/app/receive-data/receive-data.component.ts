import { Component, OnInit } from '@angular/core';
import { WritablesService, KeyValuePair } from '../writables.service';
import { Observable, of } from 'rxjs';
import { finalize, tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-receive-data',
  templateUrl: './receive-data.component.html',
  styleUrls: ['./receive-data.component.css']
})
export class ReceiveDataComponent implements OnInit {
  dyn: DynamicControllers;
  constructor(private service: WritablesService) {
    this.dyn = new DynamicControllers(service);
  }

  ngOnInit() {
    this.dyn.onInit().subscribe(it => window.alert(`Number loaded ${it}`));
  }
  save(item: KeyValuePair) {
    this.dyn.save(item).subscribe(
      () => window.setTimeout(function() { window.location.reload(); }, 3000)
    );

  }
  addNew() {
   // window.alert(this.dyn.kv.length);
    this.dyn.addNew();
   // window.alert(this.dyn.kv.length);
  }
}

export class DynamicControllers {
  kv = new Array<KeyValuePair>();

  constructor(private service: WritablesService) {}

  onInit(): Observable<number> {
    const self = this;
    return this.service.GetAll().pipe(
      tap(it => (self.kv = it)),

      switchMap(it => of(it.length))
    );
  }
  save(item: KeyValuePair): Observable<any> {
    return this.service.Save(item);
  }
  private newGuid(): string {
    let result: string;
    let i: string;
    let j: number;

    result = '';
    for (j = 0; j < 32; j++) {
      if (j === 8 || j === 12 || j === 16 || j === 20) {
        result = result + '';
      }
      i = Math.floor(Math.random() * 16)
        .toString(16)
        .toUpperCase();
      result = result + i;
    }
    return result;
  }
  addNew() {
    const kv = new KeyValuePair();
    kv.key = this.newGuid();
    kv.value = `using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    namespace StankinsDataWeb.Controllers
    {
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiVersion( "1.0" )]
        [ApiController]
        public class Controller_${kv.key} : ControllerBase
        {
            [HttpGet]
            public string Get()
            {
                return "Andrei" ;
            }
        }
    }`;
    this.kv.push(kv);
  }
}
