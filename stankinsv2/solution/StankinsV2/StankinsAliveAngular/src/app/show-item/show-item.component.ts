import { Component, OnInit, Input } from '@angular/core';
import { ResultWithData } from '../DTO/HubDeclaration';

@Component({
  selector: 'app-show-item',
  templateUrl: './show-item.component.html',
  styleUrls: ['./show-item.component.css']
})
export class ShowItemComponent implements OnInit {

  @Input()
  item: ResultWithData;
  constructor() { }

  ngOnInit() {

  }

}
