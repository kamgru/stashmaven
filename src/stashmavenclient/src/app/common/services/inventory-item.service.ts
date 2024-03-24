import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";


export interface IInventoryItemDetails {
    inventoryItemId: string;
    sku: string;
    name: string;
    unitOfMeasure: string;
    quantity: number;
    buyTaxRate: number;
    sellTaxRate: number;
    lastPurchasePrice: number;
    sellPrice: number;
}

@Injectable({
    providedIn: 'root'
})
export class InventoryItemService {

    constructor(
        private http: HttpClient
    ) {
    }

    public getInventoryItem(inventoryItemId: string): Observable<IInventoryItemDetails> {
        return this.http.get<IInventoryItemDetails>(`${environment.apiUrl}/api/v1/inventoryitem/${inventoryItemId}`);
    }
}
