import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class AddCatalogItemRequest {
    constructor(
        public name: string,
        public sku: string,
        public taxDefinitionId: string,
        public unitOfMeasure: string,
    ) {
    }
}

export interface AddCatalogItemResponse {
    value: string;
}

@Injectable({
    providedIn: 'root'
})
export class AddCatalogItemService {

    constructor(
        private http: HttpClient,
    ) {
    }

    add(req: AddCatalogItemRequest): Observable<AddCatalogItemResponse> {
        return this.http.post<AddCatalogItemResponse>('http://localhost:5253/api/v1/catalogitem', req);
    }

}
