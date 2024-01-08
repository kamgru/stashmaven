import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export interface IStockpile {
    stockpileId: string;
    name: string;
    shortCode: string;
}

export interface IShipmentKind {
    shipmentKindId: string;
    name: string;
    shortCode: string;
    direction: string;
}

export interface IListShipmentKindsResponse {
    items: IShipmentKind[];
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

export class AddPartnerToShipmentRequest {
    partnerId: string;

    constructor(partnerId: string) {
        this.partnerId = partnerId;
    }
}

@Injectable({
    providedIn: 'root'
})
export class ShipmentsService {

    constructor(
        private http: HttpClient,
    ) {
    }

    getDefaultStockpile(): Observable<IStockpile> {
        return this.http.get<IStockpile>('http://localhost:5253/api/v1/stockpile/default');
    }

    listShipmentKinds(): Observable<IListShipmentKindsResponse> {
        return this.http.get<IListShipmentKindsResponse>('http://localhost:5253/api/v1/shipment/shipmentkind/list');
    }

    addShipment(req: AddShipmentRequest): Observable<IAddShipmentResponse> {
        return this.http.post<IAddShipmentResponse>('http://localhost:5253/api/v1/shipment', req);
    }

    addPartnerToShipment(shipmentId: string, req: AddPartnerToShipmentRequest): Observable<void> {
        return this.http.patch<void>(`http://localhost:5253/api/v1/shipment/${shipmentId}/add-partner`, req);
    }

}
