import { Component, OnInit } from '@angular/core';
import { ResultWithData } from '../DTO/HubDeclaration';
import { HubDataService } from '../hub-data.service';

@Component({
  selector: 'app-result-failed',
  templateUrl: './result-failed.component.html',
  styleUrls: ['./result-failed.component.css']
})
export class ResultFailedComponent implements OnInit {

  public results: Map<string, ResultWithData>;
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

      self.Failed = Array.from(self.results.values())
        .filter((it: ResultWithData) => !it.aliveResult.isSuccess)
        .sort((a, b) => a.customData.name.localeCompare(b.customData.name));
    });
  }
}
