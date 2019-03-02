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

  private dyn: DynamicControllers;
  constructor(private service: WritablesService) {
    this.dyn = new DynamicControllers(service);
  }

  ngOnInit() {

    this.dyn.onInit().subscribe(it => window.alert(`Number loaded ${it}`));
  }
  save(item: KeyValuePair ) {
    this.dyn.save(item);
  }
}

export class DynamicControllers {

  kv = new  Array<KeyValuePair>();

  constructor(private service: WritablesService) { }

  onInit(): Observable<number> {
    const self = this;
    return this.service.GetAll().pipe(

      tap(it => self.kv = it),

      switchMap(it => of(it.length))

    );
  }
  save(item: KeyValuePair) {
    this.service.Save(item).subscribe(
      () => this.onInit().subscribe()
    );

  }
}
