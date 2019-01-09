import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'dsh-list-card',
  templateUrl: './list-card.component.html',
  styleUrls: ['./list-card.component.css']
})
export class ListCardComponent implements OnInit {

  constructor() {
    this.title = 'asda';
  }
  public title: string;
  ngOnInit() {
  }

}
