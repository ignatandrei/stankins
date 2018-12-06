import { Component, OnInit } from '@angular/core';
import { ResultWithData } from '../DTO/HubDeclaration';
import { HubDataService } from '../hub-data.service';

@Component({
  selector: 'app-result-success',
  templateUrl: './result-success.component.html',
  styleUrls: ['./result-success.component.css']
})
export class ResultSuccessComponent implements OnInit {
  public results: Map<string, ResultWithData>;
  public OK: ResultWithData[];
  public Failed: ResultWithData[];
  constructor(private data: HubDataService) {
    this.results = new Map<string, ResultWithData>();
  }
  ngOnInit(): void {
    const self = this;

    this.data.getData().subscribe(p => {
      // window.alert(JSON.stringify(p));
      // console.log('received ' + JSON.stringify(p));
      console.log('received' + p.customData.name);
      self.results.set(p.customData.name, p);

      self.OK = Array.from(self.results.values())
        .filter((it: ResultWithData) => it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));
    });
  }
}
