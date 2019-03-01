import { Component, OnInit } from '@angular/core';
import { WritablesService } from '../writables.service';

@Component({
  selector: 'app-receive-data',
  templateUrl: './receive-data.component.html',
  styleUrls: ['./receive-data.component.css']
})
export class ReceiveDataComponent implements OnInit {

  constructor(private service: WritablesService) { }

  ngOnInit() {
    window.alert('a');
    this.service.GetAll().subscribe(it => window.alert(it.length));
  }

}
