import {Injectable} from '@angular/core';
import * as li from "../list-items";
import {BehaviorSubject, Observable} from 'rxjs';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

export * from "./list-brands.service";

export class ListBrandsRequest implements li.IListRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'name';
    public isAscending: boolean = true;
}

export interface IListBrandsResponse extends li.IListResponse<IBrandItem> {
}

export interface IBrandItem {
    brandId: string;
    name: string;
    shortCode: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListBrandsService extends li.ListServiceBase<IBrandItem, ListBrandsRequest, IListBrandsResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListBrandsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListBrandsRequest): Observable<IListBrandsResponse> {
        return this.http.get<IListBrandsResponse>(`${environment.apiUrl}/api/v1/brand/list`, {
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
