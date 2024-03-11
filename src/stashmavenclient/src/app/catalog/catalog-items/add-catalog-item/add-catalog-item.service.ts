import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class AddCatalogItemRequest {
    constructor(
        public name: string,
        public sku: string,
        public unitOfMeasure: string,
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class AddCatalogItemService {

    constructor(
        private http: HttpClient,
    ) {
    }

    add(req: AddCatalogItemRequest): Observable<string> {
        return this.http.post<string>('http://localhost:5253/api/v1/catalogitem', req);
    }
}
