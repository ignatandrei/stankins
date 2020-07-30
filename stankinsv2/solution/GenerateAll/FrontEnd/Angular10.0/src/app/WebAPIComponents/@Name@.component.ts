@{
	var angular="@angular";
	var Component = "@Component";
	var dt= Model.FindAfterName("@Name@").Value;
	
}
import { Component, OnInit } from '@(angular)/core';
import{ @dt.TableName } from './../WebAPIClasses/@dt.TableName';

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
