import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'dsh-dashboard-simple',
  template: `
    <p>
      dashboard-simple works!
    </p>
    <dsh-dash-board></dsh-dash-board>
  `,
  styles: []
})
export class DashboardSimpleComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
