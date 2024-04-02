import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable, Subject} from "rxjs";
import {environment} from "../../../../environments/environment";
import * as li from "../list-items";

export * from "./list-products.service";

export class ListProductsRequest implements li.IListRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IListProductsResponse extends li.IListResponse<IProduct> {
}

export interface IProduct {
    productId: string;
    sku: string;
    name: string;
    unitOfMeasure: string;
    tax: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListProductsService extends li.ListServiceBase<IProduct, ListProductsRequest, IListProductsResponse> {

    constructor(
        private http: HttpClient,
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListProductsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListProductsRequest): Observable<IListProductsResponse> {
        return this.http.get<IListProductsResponse>(`${environment.apiUrl}/api/v1/product/list`, {
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
