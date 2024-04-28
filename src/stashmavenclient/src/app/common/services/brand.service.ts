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

export interface IGetBrandResponse {
    name: string;
    shortCode: string;
}

export class UpdateBrandRequest {
    constructor(
        public readonly brandId: string,
        public readonly name: string,
        public readonly shortCode: string
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

    public addBrand(req: AddBrandRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/brand`, req);
    }

    public getBrand(brandId: string): Observable<IGetBrandResponse> {
        return this.http.get<IGetBrandResponse>(`${environment.apiUrl}/api/v1/brand/${brandId}`);
    }

    public updateBrand(req: UpdateBrandRequest) {
        return this.http.put(`${environment.apiUrl}/api/v1/brand/${req.brandId}`, req);
    }
}
