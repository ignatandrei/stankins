import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { RecipesService } from '../recipes.service';
import { Recipe } from '../Recipe';
import { tap, switchMap, filter, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject, fromEvent, of, zip } from 'rxjs';
import { FormControl } from '@angular/forms';
import { SearchRecipe } from './SearchRecipe';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.css'],
  // changeDetection: ChangeDetectionStrategy.OnPush,
})


export class RecipesListComponent implements OnInit {

  items = Array.from({length: 100000}).map((_, i) => `Item #${i}`);
  search: SearchRecipe;
  numberRecipes: number;
  findRecipes: Recipe[];
  searchRecipe = new FormControl();

  constructor(private recipeService: RecipesService, private cd: ChangeDetectorRef, private route: ActivatedRoute,
    private router: Router) {

  }

  ngOnInit() {
    const self = this;
    self.search = new SearchRecipe(this.recipeService);
    const search$ = self.search.loadRecipes();
    const id$ =
    this.route.queryParamMap.pipe(
      switchMap((params) => of(params.get('id')))
    );

    zip(search$, id$).subscribe(([nr, id]) => {

        self.numberRecipes = nr;
        if (id == null || id.length === 0) {
          self.findRecipes = this.search.FileRecipes();
        } else {
          self.searchRecipe.setValue(id);
        }
        // window.alert('finished loading');
        // self.cd.detectChanges();
      });


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


