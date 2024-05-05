import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export class AddBusinessIdentifierRequest {
    constructor(
        public readonly shortCode: string,
        public readonly name: string,
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class BusinessIdentifierService {

    constructor(
        private http: HttpClient
    ) {
    }

    public addBusinessIdentifier(req: AddBusinessIdentifierRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/businessidentifier`, req);
    }
}
