import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';
import { Recipe } from '../Recipe';
import { SearchRecipe } from "../recipes-list/SearchRecipe";
import {  of, zip } from 'rxjs';
import { RecipesService } from '../recipes.service';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  selectedRecipe: Recipe;
  search: SearchRecipe;
  nr: Number;
  constructor( private route: ActivatedRoute,
    private router: Router, private recipeService: RecipesService) {
      this.search = new SearchRecipe(recipeService);

    }

  ngOnInit() {
    const self = this;
    const id$ =
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => of(params.get('id')))
    );
    const searchN = this.search.loadRecipes();
    zip(id$, searchN).subscribe( val => {
      self.selectedRecipe = this.search.SearchRecipeByName(val[0]);
      self.nr = val[1];
      if (self.selectedRecipe.arguments == null) {
        self.execute();
      }
    });


  }
  execute() {
     this.search.execute(this.selectedRecipe).subscribe(it => console.log('done'+ it));

  }

}
