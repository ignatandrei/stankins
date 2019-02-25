import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap, tap, finalize } from 'rxjs/operators';
import { Recipe } from '../Recipe';
import { SearchRecipe, RowConfiguration } from '../recipes-list/SearchRecipe';
import {  of, zip } from 'rxjs';
import { RecipesService } from '../recipes.service';
export enum WhatToOpen {
  None= 0,
  Definition = 1,
  Results = 2
}
@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {
  public executing = false;
  public WhatToOpen: any = WhatToOpen;
  whatToOpen = WhatToOpen.None;
  selectedRecipe: Recipe;
  search: SearchRecipe;
  nr: Number;
  executed = new Map<string, [string[], RowConfiguration[]]>();
  constructor( private route: ActivatedRoute,
    private router: Router, private recipeService: RecipesService) {
      this.search = new SearchRecipe(recipeService);

    }

  ngOnInit() {

    const self = this;
    self.whatToOpen = WhatToOpen.Definition;
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
  nameTables(): string[] {
    return Array.from(this.executed.keys());
  }
  execute() {
    const self = this;
    self.executing = true;
    self.executed = new Map<string, [string[], RowConfiguration[]]>();
    self.whatToOpen = WhatToOpen.Definition;
     this.search.execute(this.selectedRecipe)
      .pipe(
        finalize(() => {
          self.executing = false;
          self.whatToOpen = WhatToOpen.Results;
        })
      )
      .subscribe(it => {

       // console.log('done ' + it.length );
       if (it.length > 1) {
        const tableName = it.shift();
        // console.log(it.length);
        self.executed.set(tableName, self.search.TransformToRow(it));
        // console.table(self.executed.get(tableName)[0]);
        // console.table(self.executed.get(tableName)[1]);
       }
    });


  }

}
