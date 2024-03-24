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

export class AddRecordToShipmentRequest{
    constructor(
        public inventoryItemId: string,
        public quantity: number,
        public unitPrice: number
    ) {
    }

}
export interface IGetShipmentResponse {
    partner: IShipmentPartner;
    records: IShipmentRecord[];
    kind: IShipmentKindInfo;
    currency: string;
}

export interface IShipmentKindInfo {
    name: string;
    shortCode: string;
    direction: string;
}

export interface IShipmentRecord {
    inventoryItemId: string;
    quantity: number;
    unitPrice: number;
    sku: string;
    name: string;
    taxRate: number;
}

export interface IShipmentPartner {
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

    getShipment(shipmentId: string): Observable<IGetShipmentResponse> {
        return this.http.get<IGetShipmentResponse>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`);
    }

    deleteShipment(shipmentId: string): Observable<void> {
        return this.http.delete<void>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`);
    }

    updateShipment(shipmentId: string, req: UpdateShipmentRequest): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/shipment/${shipmentId}`, req);
    }
    public addPartnerToShipment(shipmentId: string, partnerId: string) {
        return this.http.patch(`${environment.apiUrl}/api/v1/shipment/${shipmentId}/partner`, {partnerId});
    }

    public addRecordToShipment(shipmentId: string, req: AddRecordToShipmentRequest) {
        return this.http.patch(`${environment.apiUrl}/api/v1/shipment/${shipmentId}/record`, req);
    }

    public acceptShipment(shipmentId: string) {
        return this.http.post(`${environment.apiUrl}/api/v1/shipment/${shipmentId}/accept`, {});
    }
}
