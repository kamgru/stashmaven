import {Injectable} from '@angular/core';
import {
    ISuperGridListService,
    SuperGridListRequest,
    SuperGridListResponse
} from "../../common/super-grid/super-grid-list-service";
import {Observable} from 'rxjs';
import {HttpClient} from "@angular/common/http";

export interface InventoryItem {
    inventoryItemId: string,
    name: string,
    sku: string,
    purchasePrice: number,
    quantity: number,
    taxRate: number,
}

export class ListInventoryItemsRequest extends SuperGridListRequest {
    stockpileId: string;

    constructor(
        stockpileId: string,
        page: number,
        pageSize: number,
        search?: string,
        sortBy?: string,
        isAscending: boolean = true) {
        super(page, pageSize, search, sortBy, isAscending);
        this.stockpileId = stockpileId;
    }
}

@Injectable({
    providedIn: 'root'
})
export class ListInventoryItemsService implements ISuperGridListService<InventoryItem> {

    constructor(
        private http: HttpClient
    ) {
    }

    list(req: SuperGridListRequest): Observable<SuperGridListResponse<InventoryItem>> {
        return this.http.get<SuperGridListResponse<InventoryItem>>('http://localhost:5253/api/v1/inventory/inventory-item/list', {params: req.toHttpParams()});
    }
}
