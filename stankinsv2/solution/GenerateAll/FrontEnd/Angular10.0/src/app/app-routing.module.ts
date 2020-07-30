@{
  var angular="@angular";
  var NgModule="@NgModule";
  
  var ds= Model.FindAfterName("DataSource").Value;
    
    var nrRowsDS=ds.Rows.Count;
    
    var nameTablesToRender = new string[nrRowsDS];
    var tables=new System.Data.DataTable[nrRowsDS];
    for (int iRowDS = 0; iRowDS < nrRowsDS; iRowDS++)
    {
        var nameTable = ds.Rows[iRowDS]["TableName"].ToString();
        var renderTable = Model.FindAfterName(nameTable).Value;
        nameTablesToRender[iRowDS] = nameTable;
        tables[iRowDS]=renderTable;
    }

}

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AboutComponent } from './about/about.component';


@foreach(var nameTable in nameTablesToRender){
<text>
import { @(nameTable)Component } from './WebAPIComponents/@(nameTable).Component';

</text>
}


const routes: Routes = [
  { path: 'about', component: AboutComponent },
@foreach(var nameTable in nameTablesToRender){
<text>
{ path: '@(nameTable)', component: @(nameTable)Component },

</text>
}
  

];

@(NgModule)({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
