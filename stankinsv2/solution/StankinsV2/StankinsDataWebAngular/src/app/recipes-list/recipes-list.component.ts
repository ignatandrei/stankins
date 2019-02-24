import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { RecipesService } from '../recipes.service';
import { Recipe } from '../Recipe';
import { tap, switchMap, filter, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject, fromEvent } from 'rxjs';
import { FormControl } from '@angular/forms';
import { SearchRecipe } from './SearchRecipe';
@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})


export class RecipesListComponent implements OnInit {

  items = Array.from({length: 100000}).map((_, i) => `Item #${i}`);
  search: SearchRecipe;
  numberRecipes: number;
  findRecipes: Recipe[];
  searchRecipe = new FormControl();

  constructor(private recipeService: RecipesService) {

  }

  ngOnInit() {
    this.search = new SearchRecipe(this.recipeService);
    this.search.loadRecipes().subscribe(it => this.numberRecipes = it);

  this.searchRecipe.valueChanges.pipe(
    // filter(it => it != null && it.length > 1),
    debounceTime(100),
    distinctUntilChanged(),
    tap(it => {

      console.log(it);

    })
  ).subscribe(it => this.findRecipes = this.search.SearchRecipe(it)  );
  }

}


