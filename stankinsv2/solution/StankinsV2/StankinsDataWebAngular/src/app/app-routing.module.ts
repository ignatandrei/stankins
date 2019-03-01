import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RecipesListComponent } from './recipes-list/recipes-list.component';
import { RecipeComponent } from './recipe/recipe.component';
import { VersionsComponent } from './versions/versions.component';
import { ReceiveDataComponent } from './receive-data/receive-data.component';

const routes: Routes = [
  {path: 'recipeList', component: RecipesListComponent},
  {path: '', redirectTo: '/recipeList', pathMatch: 'full'},
  { path: 'recipe/:id',      component: RecipeComponent },
  { path: 'about',      component: VersionsComponent},
  {path: 'receiveData', component: ReceiveDataComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
