import { NgModule } from '@angular/core';
import { VersionsComponent } from './versions.component';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [VersionsComponent],
  imports: [
    CommonModule
  ],
  exports: [VersionsComponent]
})
export class VersionsModule { }
