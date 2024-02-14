import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

export class AddRecordToShipmentRequest{
    constructor(
        public shipmentId: string,
        public inventoryItemId: string,
        public quantity: number,
        public unitPrice: number
    ) {
    }
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
        return this.http.post(`${environment.apiUrl}/api/v1/shipment/${req.shipmentId}/record`, {
            inventoryItemId: req.inventoryItemId,
            quantity: req.quantity,
            unitPrice: req.unitPrice
        });
    }
}
