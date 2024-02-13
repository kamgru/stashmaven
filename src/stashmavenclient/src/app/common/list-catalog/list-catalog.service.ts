import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable, Subject} from "rxjs";
import {environment} from "../../../environments/environment";
import * as li from "../components/list-items";

export * from "./list-catalog.service";

export class ListCatalogItemsRequest implements li.IListRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IListCatalogItemsResponse extends li.IListResponse<ICatalogItem> {
}

export interface ICatalogItem {
    catalogItemId: string;
    sku: string;
    name: string;
    unitOfMeasure: string;
    tax: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListCatalogService extends li.ListServiceBase<ICatalogItem, ListCatalogItemsRequest, IListCatalogItemsResponse> {

    constructor(
        private http: HttpClient,
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListCatalogItemsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListCatalogItemsRequest): Observable<IListCatalogItemsResponse> {
        return this.http.get<IListCatalogItemsResponse>(`${environment.apiUrl}/api/v1/catalogitem/list`, {
            params: {
                page: request.page,
                pageSize: request.pageSize,
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending,
            }
        });
    }
}
