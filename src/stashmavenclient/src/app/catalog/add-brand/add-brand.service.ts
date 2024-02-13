import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AddBrandService {

    constructor(
        private http: HttpClient,
    ) {
    }

    addBrand(name: string, shortCode: string) : Observable<string> {
        return this.http.post<string>('http://localhost:5253/api/v1/brand', {
            name: name,
            shortCode: shortCode
        });
    }
}
