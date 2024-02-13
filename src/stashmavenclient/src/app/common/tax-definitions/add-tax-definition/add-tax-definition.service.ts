import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class AddTaxDefinitionRequest {
    constructor(
        public name: string,
        public rate: number
    ) {
    }
}

export interface AddTaxDefinitionResponse {
    value: string;
}

@Injectable({
    providedIn: 'root'
})
export class AddTaxDefinitionService {

    constructor(
        private http: HttpClient
    ) {
    }

    add(req: AddTaxDefinitionRequest): Observable<AddTaxDefinitionResponse> {
        return this.http.post<AddTaxDefinitionResponse>('http://localhost:5253/api/v1/taxdefinition', req);
    }
}
