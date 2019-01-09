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
  tiles = [
    {text: 'One', cols: 1, rows: 1, color: '#142A5C'},
    {text: 'Two', cols: 1, rows: 1, color: '#B7A0E8'},
    {text: 'Three', cols: 1, rows: 1, color: '#FF0000'},
    {text: 'Four', cols: 1, rows: 1, color: '#D9EDD9'},
  ];
  ngOnInit() {
  }

}
