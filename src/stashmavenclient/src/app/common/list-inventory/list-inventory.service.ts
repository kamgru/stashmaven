import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";

export class ListInventoryItemsRequest implements IListRequest {
    public stockpileId: string = '';
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IInventoryItem {
    inventoryItemId: string;
    sku: string;
    name: string;
    quantity: number;
    unitOfMeasure: string;
    lastPurchasePrice: number;
}

export interface IListInventoryItemsResponse extends IListResponse<IInventoryItem> {
    stockpileId: string;
}

export interface IGetDefaultStockpileIdResponse {
    stockpileId: string;
    name: string;
    shortCode: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListInventoryService extends ListServiceBase<IInventoryItem, ListInventoryItemsRequest, IListInventoryItemsResponse> {

    constructor(
        private http: HttpClient,
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListInventoryItemsRequest());
        this.bootstrap();
    }

    override listItems(request: ListInventoryItemsRequest): Observable<IListInventoryItemsResponse> {
        return this.http.get<IListInventoryItemsResponse>(`${environment.apiUrl}/api/v1/inventoryitems/list`, {
            params: {
                stockpileId: request.stockpileId,
                page: request.page.toString(),
                pageSize: request.pageSize.toString(),
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending.toString()
            }
        });
    }

    changeStockpile(stockpileId: string) {
        this._request$.next({...this._request$.value, stockpileId, page: 1});
    }
}
