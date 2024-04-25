import * as li from "../list-items";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "../../../../environments/environment";
import {Injectable} from "@angular/core";

export * from "./list-stockpiles.service";

export class ListStockpilesRequest implements li.IListRequest {
    page: number = 1;
    pageSize: number = 10;
    search: string = '';
    sortBy: string = '';
    isAscending: boolean = true;
}

export interface IStockpile {
    stockpileId: string;
    shortCode: string;
    name: string;
    isDefault: boolean;
}

export interface IListStockpilesResponse extends li.IListResponse<IStockpile> {
    stockpileId: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListStockpilesService extends li.ListServiceBase<IStockpile, ListStockpilesRequest, IListStockpilesResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListStockpilesRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListStockpilesRequest): Observable<IListStockpilesResponse> {
        return this.http.get<IListStockpilesResponse>(`${environment.apiUrl}/api/v1/stockpile/list`, {
            params: {
                page: request.page.toString(),
                pageSize: request.pageSize.toString(),
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending.toString()
            }
        });
    }
}