import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  name:string;
  constructor( private route: ActivatedRoute,
    private router: Router) { 


    }

  ngOnInit() {
    this.route.paramMap.pipe(
      tap((params: ParamMap) =>
        this.name = params.get('id')))
        .subscribe();
    ;
  }

}
