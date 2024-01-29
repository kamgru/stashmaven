import {Injectable} from '@angular/core';

class ListInventoryRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface InventoryItem {
    inventoryItemId: string;
    sku: string;
    name: string;
    quantity: number;
    unitOfMeasure: string;
    lastPurchasePrice: number;
}

export interface ListInventoryResponse {
    items: InventoryItem[];
    totalCount: number;
}

@Injectable({
    providedIn: 'root'
})
export class ListInventoryService {

    constructor() {
    }
}
