import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FileVersionInfo } from './FileVersion';

@Injectable({
  providedIn: 'root'
})
export class VersionsService {

  constructor(private http: HttpClient) {
    const self = this;

  }

  public FVS(): Observable<FileVersionInfo[]> {
    console.log('get fvs');
    return this.http.get<FileVersionInfo[]>('api/Utils/GetVersions');
  }
}
