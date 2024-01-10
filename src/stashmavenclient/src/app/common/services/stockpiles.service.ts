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
}

export interface InventoryItemListItem {
    inventoryItemId: string;
    name: string;
    sku: string;
    unit: string;
    quantity: number;
    unitOfMeasure: string;
    lastPurchasePrice: number;
}

export interface ListInventoryItemsResponse {
    items: InventoryItemListItem[];
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

    listInventoryItems(stockpileId: string, req: ListInventoryItemsRequest): Observable<ListInventoryItemsResponse> {
        return this.http.get<ListInventoryItemsResponse>(`http://localhost:5253/api/v1/stockpile/${stockpileId}/inventory-items`);
    }
}
