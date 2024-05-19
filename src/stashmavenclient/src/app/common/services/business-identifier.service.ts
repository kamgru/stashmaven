import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export interface IBusinessIdentifierType {
    id: string;
    name: string;
    countryCode: string;
    isPrimary: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class BusinessIdentifierService {

    constructor(
        private http: HttpClient
    ) {
    }

    public getBusinessIdentifierTypes(): Observable<IBusinessIdentifierType[]> {
        return this.http.get<IBusinessIdentifierType[]>(`${environment.apiUrl}/api/v1/businessidentifier/types`);
    }
}
