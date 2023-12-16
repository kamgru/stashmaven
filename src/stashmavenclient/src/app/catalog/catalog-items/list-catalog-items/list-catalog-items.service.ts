import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class ListCatalogItemsRequest {
    constructor(
        public page: number,
        public pageSize: number,
        public sortBy: string | null,
        public isAscending: boolean,
        public search: string | null,
    ) {
    }
}

export interface ICatalogItem {
    catalogItemId: string;
    name: string;
    sku: string;
    tax: string;
    unitOfMeasure: string;
}

export interface ListCatalogItemsResponse {
    items: ICatalogItem[];
    totalCount: number;
}

@Injectable({
    providedIn: 'root'
})
export class ListCatalogItemsService {

    constructor(
        private http: HttpClient
    ) {
    }

    list(req: ListCatalogItemsRequest): Observable<ListCatalogItemsResponse> {
        return this.http.get<ListCatalogItemsResponse>('http://localhost:5253/api/v1/catalogitem/list',
            {
                params: {
                    page: req.page.toString(),
                    pageSize: req.pageSize.toString(),
                    sortBy: req.sortBy || '',
                    isAscending: req.isAscending.toString(),
                    search: req.search || '',
                }
            });
    }
}
