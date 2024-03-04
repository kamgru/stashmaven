import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export interface IAvailableCountry {
    name: string;
    code: string;
}

export class AddAvailableCountryRequest {
    name: string;
    code: string;

    constructor(name: string, code: string) {
        this.name = name;
        this.code = code;
    }
}

@Injectable({
    providedIn: 'root'
})
export class CountryService {

    constructor(
        private http: HttpClient
    ) {
    }

    public getAvailableCountries(): Observable<IAvailableCountry[]> {
        return this.http.get<IAvailableCountry[]>(`${environment.apiUrl}/api/v1/country/available`);
    }

    public addAvailableCountry(req: AddAvailableCountryRequest): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/v1/country/available`, req);
    }
}
