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
}
