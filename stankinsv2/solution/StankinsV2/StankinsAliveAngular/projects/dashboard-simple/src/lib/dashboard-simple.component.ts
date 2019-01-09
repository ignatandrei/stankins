import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'dsh-dashboard-simple',
  template: `
    <p>
      dashboard-simple works!
    </p>
    <dsh-list-card></dsh-list-card>
  `,
  styles: []
})
export class DashboardSimpleComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
