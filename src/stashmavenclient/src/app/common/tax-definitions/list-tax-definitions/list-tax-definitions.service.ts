import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export interface TaxDefinition {
    taxDefinitionId: string;
    name: string;
    rate: number;
}

export interface ListTaxDefinitionsResponse {
    items: TaxDefinition[];
    totalCount: number;
}

@Injectable({
    providedIn: 'root'
})
export class ListTaxDefinitionsService {

    constructor(
        private http: HttpClient
    ) {
    }

    listAll(): Observable<ListTaxDefinitionsResponse> {
        return this.http.get<ListTaxDefinitionsResponse>(
            'http://localhost:5253/api/v1/taxdefinition/list', {
                params: {page: '1', size: '100'}
            });
    }
}
