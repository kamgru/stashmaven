import {Injectable} from "@angular/core";
import {catchError, Observable} from "rxjs";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {environment} from "../../../environments/environment";

export interface TaxDefinition {
    taxDefinitionId: string;
    name: string;
    rate: number;
    countryCode: string;
}

export interface ListTaxDefinitionsResponse {
    items: TaxDefinition[];
    totalCount: number;
}

export class AddTaxDefinitionRequest {
    constructor(
        public name: string,
        public rate: number,
    ) {
    }
}

export interface AddTaxDefinitionResponse {
    value: string;
}

@Injectable({
    providedIn: 'root'
})
export class TaxDefinitionService {

    constructor(
        private http: HttpClient
    ) {
    }

    listAll(): Observable<ListTaxDefinitionsResponse> {
        return this.http.get<ListTaxDefinitionsResponse>(
            `${environment.apiUrl}/api/v1/taxdefinition/list`);
    }

    addTaxDefinition(req: AddTaxDefinitionRequest): Observable<AddTaxDefinitionResponse> {
        return this.http.post<AddTaxDefinitionResponse>(`${environment.apiUrl}/api/v1/taxdefinition`, req);
    }

    patch(id: string, name: string): Observable<void> {
        return this.http.patch<void>(
            `${environment.apiUrl}/api/v1/taxdefinition/` + id, {name: name});
    }

    tryDelete(id: string): Observable<void> {
        return this.http.delete<void>(
            `${environment.apiUrl}/api/v1/taxdefinition/` + id)
            .pipe(
                catchError((err: HttpErrorResponse) => {
                    throw new Error(err.error);
                })
            );
    }
}