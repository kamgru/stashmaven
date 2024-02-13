import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {catchError, Observable} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class EditTaxDefinitionService {

    constructor(
        private http: HttpClient
    ) {
    }

    patch(id: string, name: string): Observable<void> {
        return this.http.patch<void>(
            'http://localhost:5253/api/v1/taxdefinition/' + id, {name: name});
    }

    tryDelete(id: string): Observable<void> {
        return this.http.delete<void>(
            'http://localhost:5253/api/v1/taxdefinition/' + id)
            .pipe(
                catchError((err: HttpErrorResponse) => {
                   throw new Error(err.error);
                })
            );
    }
}
