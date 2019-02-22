import { Component, OnInit } from '@angular/core';
import { RecipesService } from '../recipes.service';

@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.css']
})
export class RecipesListComponent implements OnInit {

  constructor(private recipeService: RecipesService) { }

  ngOnInit() {
    this.recipeService.GetStankinsAll().subscribe(it=> window.alert(it.length));
  }

}
