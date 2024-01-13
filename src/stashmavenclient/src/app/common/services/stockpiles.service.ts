import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class ListInventoryItemsRequest {
    page: number;
    pageSize: number;
    search?: string;
    sortBy?: string;
    isAscending: boolean = true;

    constructor(page: number, pageSize: number, search?: string, sortBy?: string, isAscending?: boolean) {
        this.page = page;
        this.pageSize = pageSize;
        this.search = search;
        this.sortBy = sortBy;
        this.isAscending = isAscending || true;
    }

    toHttpParams() {
        return {
            page: this.page,
            pageSize: this.pageSize,
            search: this.search ?? '',
            sortBy: this.sortBy ?? '',
            isAscending: this.isAscending
        };
    }
}

export interface IInventoryItem {
    inventoryItemId: string;
    name: string;
    sku: string;
    unit: string;
    quantity: number;
    unitOfMeasure: string;
    lastPurchasePrice: number;
}

export interface IListInventoryItemsResponse {
    items: IInventoryItem[];
    totalCount: number;
}


@Injectable({
    providedIn: 'root'
})
export class StockpilesService {

    constructor(
        private http: HttpClient
    ) {
    }

    listInventoryItems(stockpileId: string, req: ListInventoryItemsRequest): Observable<IListInventoryItemsResponse> {
        return this.http.get<IListInventoryItemsResponse>(`http://localhost:5253/api/v1/stockpile/${stockpileId}/inventory-items`, {params: req.toHttpParams()});
    }
}
