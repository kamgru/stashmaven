import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable, Subject} from "rxjs";
import {environment} from "../../../environments/environment";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";

export class ListCatalogItemsRequest implements IListRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IListCatalogItemsResponse extends IListResponse<ICatalogItem> {
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
export class ListCatalogService extends ListServiceBase<ICatalogItem, ListCatalogItemsRequest, IListCatalogItemsResponse> {

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
