@{
	var angular="@angular";
	var Component = "@Component";
}
import { Component, OnInit } from '@(angular)/core';

@(Component)({
  selector: 'app-@Name@-component',
  templateUrl: './@Name@.component.html',
  styleUrls: ['./@Name@.component.css']
})
export class @Name@Component implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
