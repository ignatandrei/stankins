@{
	var angular="@angular";
	var Component = "@Component";
	var dt= Model.FindAfterName("@Name@").Value;
	var nameTable =dt.TableName;
}
import { Component, OnInit } from '@(angular)/core';
import{ @nameTable } from './../WebAPIClasses/@nameTable';
import {@(nameTable)Service} from './../services/@(nameTable).service';

@(Component)({
  selector: 'app-@Name@-component',
  templateUrl: './@Name@.component.html',
  styleUrls: ['./@Name@.component.css']
})
export class @Name@Component implements OnInit {

  constructor(private mainService: @(nameTable)Service ) { }

  ngOnInit(): void {
	  this.mainService.GetAll().subscribe(it=>{
      window.alert(it.length);
    })
  }

}
