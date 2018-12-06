import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AboutComponent } from './about/about.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ResultSuccessComponent } from './result-success/result-success.component';
import { ResultFailedComponent } from './result-failed/result-failed.component';
import { TagDisplayComponent } from './tag-display/tag-display.component';

const routes: Routes = [
  {
    path: 'about',
    component: AboutComponent
  },
  { path: 'dashboard', component: DashboardComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'resultSuccess', component: ResultSuccessComponent },
  { path: 'resultFailed', component: ResultFailedComponent },
  { path: 'tag/:id', component: TagDisplayComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
