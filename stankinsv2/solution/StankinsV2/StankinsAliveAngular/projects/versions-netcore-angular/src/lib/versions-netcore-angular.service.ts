import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, pipe, of } from 'rxjs';
import { FileVersionInfo, FVSAngular } from './FileVersion';
import { map, switchMap } from 'rxjs/operators';
import { strictEqual } from 'assert';
@Injectable({
    providedIn: 'root'
})
export class VersionsNetcoreAngularService {
    constructor(private http: HttpClient) {
        const self = this;
    }
    public FVS(): Observable<FileVersionInfo[]> {
        console.log('get fvs');
        return this.http.get<FileVersionInfo[]>('api/Utils/GetVersions');
    }
    public FVSAngular(): Observable<FVSAngular[]> {
        console.log('get fvs angular');
        return  this.http.get('npm-shrinkwrap.json').pipe
        (
            switchMap(data => {
                window.alert(JSON.stringify(data));
                const arr = new Array<FVSAngular>();
                const f = new FVSAngular();
                f.dev = false;
                f.name = data['name'];
                f.version = data['version'];
                arr.push(f);

                // const deps = data['dependencies'] as Map<string, any>;

                // for (let index = 0; index < deps.keys.length; index++) {
                //     const element = deps[deps.keys[index]];
                //     const newF = new FVSAngular();
                //     newF.version = element['version'];
                //     newF.name =  deps.keys[index];
                //     arr.push(newF);
                // }
                return of(arr);
           })
        );

    }
}
