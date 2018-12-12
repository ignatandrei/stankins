import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultWithData } from '../DTO/HubDeclaration';
import { HubDataService } from '../hub-data.service';

@Component({
  selector: 'app-tag-display',
  templateUrl: './tag-display.component.html',
  styleUrls: ['./tag-display.component.css']
})
export class TagDisplayComponent implements OnInit {

  public results: Map<string, ResultWithData>;
  public DisplayResults: ResultWithData[];
  public id: string;
  public OK: ResultWithData[];
  public Failed: ResultWithData[];
  constructor(private route: ActivatedRoute, private data: HubDataService) {
    route.params.subscribe(val => this.ngOnInit());
    // or add Router and put routereusestragegy 
    // or  https://medium.com/@juliapassynkova/angular-2-component-reuse-strategy-9f3ddfab23f5
   }

  ngOnInit() {
    const self = this;
    this.id = this.route.snapshot.paramMap.get('id');
    this.results = new Map<string, ResultWithData>();
    console.log(this.id);
    this.data.getData().subscribe(p => {
      // window.alert(JSON.stringify(p));
      // console.log('received ' + JSON.stringify(p));
      // console.log('received' + p.customData.name);
      self.results.set(p.customData.name, p);

      self.DisplayResults = Array.from(self.results.values())
        .filter((it: ResultWithData) => it.customData.tags.some(t => t === self.id))
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));

      self.OK = Array.from(self.DisplayResults)
        .filter((it: ResultWithData) => it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));

      self.Failed = Array.from(self.DisplayResults)
        .filter((it: ResultWithData) => !it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));

    });
  }

}
