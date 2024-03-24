import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";

export interface IListShipmentKindsResponse {
    items: IShipmentKind[];
}

export interface IShipmentKind {
    shipmentKindId: string;
    name: string;
    shortCode: string;
    direction: string;
}

export class AddShipmentRequest {
    constructor(
        public stockpileId: string,
        public shipmentKindId: string,
        public currency: string,
    ) {
    }
}

export interface IAddShipmentResponse {
    shipmentId: string;
}

export interface IShipmentEditDetails {
    partner: IShipmentPartnerEditDetails;
    records: IShipmentRecordEditDetails[];
    kind: IShipmentKindEditDetails;
    currency: string;
}

export interface IShipmentKindEditDetails {
    name: string;
    shortCode: string;
    direction: string;

}

export interface IShipmentRecordEditDetails {
    inventoryItemId: string;
    quantity: number;
    unitPrice: number;
    sku: string;
    name: string;
    taxRate: number;
}

export interface IShipmentPartnerEditDetails {
    partnerId: string;
    legalName: string;
    customIdentifier: string;
    address: string;
}

export class UpdateShipmentRequest {
    constructor(
        public sourceReferenceIdentifier: string,
        public issuedOn: string
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class ShipmentService {

    constructor(
        private http: HttpClient
    ) {
    }

    listShipmentKinds(): Observable<IListShipmentKindsResponse> {
        return this.http.get<IListShipmentKindsResponse>(`${environment.apiUrl}/api/v1/shipment/shipmentKind/list`);
    }

    addShipment(req: AddShipmentRequest): Observable<IAddShipmentResponse> {
        return this.http.post<IAddShipmentResponse>(`${environment.apiUrl}/api/v1/shipment`, req);
    }

    getShipment(shipmentId: string): Observable<IShipmentEditDetails> {
        return this.http.get<IShipmentEditDetails>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`);
    }

    deleteShipment(shipmentId: string): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`);
    }

    updateShipment(shipmentId: string, req: UpdateShipmentRequest): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`, req);
    }
}
