import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {Observable} from "rxjs";

export class AddRecordToShipmentRequest{
    constructor(
        public shipmentId: string,
        public inventoryItemId: string,
        public quantity: number,
        public unitPrice: number
    ) {
    }
}

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
export class EditShipmentService {

    constructor(
        private http: HttpClient
    ) {
    }

    public addPartnerToShipment(shipmentId: string, partnerId: string) {
        return this.http.patch(`${environment.apiUrl}/api/v1/shipment/${shipmentId}/partner`, {partnerId});
    }

    public addRecordToShipment(req: AddRecordToShipmentRequest) {
        console.log(req);
        return this.http.patch(`${environment.apiUrl}/api/v1/shipment/${req.shipmentId}/record`, {
            inventoryItemId: req.inventoryItemId,
            quantity: req.quantity,
            unitPrice: req.unitPrice
        });
    }

    public getInventoryItem(inventoryItemId: string): Observable<IInventoryItemDetails> {
        return this.http.get<IInventoryItemDetails>(`${environment.apiUrl}/api/v1/inventoryitem/${inventoryItemId}`);
    }

    public acceptShipment(shipmentId: string) {
        return this.http.post(`${environment.apiUrl}/api/v1/shipment/${shipmentId}/accept`, {});
    }
}
