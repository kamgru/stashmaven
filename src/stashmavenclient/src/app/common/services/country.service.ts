import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export interface IAvailableCountry {
    name: string;
    code: string;
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
}
