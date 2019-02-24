import { RecipesService } from '../recipes.service';
import { Recipe } from '../Recipe';
import { tap, map, switchMap, flatMap, concatMapTo, concatMap, mergeMap, scan, switchAll, combineAll, mergeMapTo } from 'rxjs/operators';
import { Observable, from, combineLatest, forkJoin } from 'rxjs';
export class SearchRecipe {
  // private findRecipes=new  Subject<number>();
  private allRecipes: Recipe[];
  constructor(private recipeService: RecipesService) {
  }
  loadRecipes(): Observable<number> {
    return this.recipeService.GetStankinsAll()
      .pipe(tap(it => {
        window.console.table(it);
      }), map((rec) => {
        const list = new Array<Recipe>();
        rec.forEach(element => {
          list.push(new Recipe(element));
        });
        this.allRecipes = list.sort((x, y) => x.name.localeCompare(y.name));
        return list.length;
      }));
    // .subscribe(it => {
    //     this.allRecipes = it;
    //     this.findRecipes.next(it.length);
    //   });
  }
  // public findRecs(): Observable<number> {
  //   return this.findRecipes.asObservable();
  // }
  public SearchRecipe(s: string): Recipe[] {
    if (s == null || s.length === 0) {
      return null;
    }
    return this.allRecipes.filter(it => it.searchString().indexOf(s) > -1);
  }
  public SearchRecipeByName(s: string): Recipe {
    s = s.toLowerCase();
    return this.allRecipes.find(it => it.name.toLowerCase() === s);
  }
  public execute(r: Recipe): Observable<string[]> {

    const ret1 =  this.recipeService.execute(r).pipe
    (
        tap(id => {
            console.log(`getting ${id} `);

        } ),

        mergeMap(it =>
            this.recipeService.getTables(it).pipe(
                map($ => $.map(table => ({table, it}) ))
            )

        ),

        mergeMap(vals =>
            vals.map(val => this.recipeService.getTablesValues(val.it, val.table)
                .pipe(
                    map($ => [val.table, ...$])
                )
            )),


        combineAll(),
        mergeMap(val => val),


    );


    return ret1;
  }
}
