import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {Observable} from "rxjs";

export class AddBrandRequest{
    constructor(
        public name: string,
        public shortCode: string
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class BrandService {

    constructor(
        private http: HttpClient
    ) {
    }

    public addBrand(request: AddBrandRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/brand`, request);
    }
}
