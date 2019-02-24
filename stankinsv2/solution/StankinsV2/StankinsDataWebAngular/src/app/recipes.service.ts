import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe } from './Recipe';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RecipesService {

  constructor(private http: HttpClient) { }
  public GetStankinsAll(): Observable<Recipe[]> {
    let url = environment.url;
    url += '/api/v1.0/recipes';
    return this.http.get<Recipe[]>(url);

  }

  public execute(r: Recipe): Observable<string> {
    let url = environment.url;
    url += '/api/v1.0/recipes';
    return this.http.post(url, r , { responseType: 'text'});
  }
}



