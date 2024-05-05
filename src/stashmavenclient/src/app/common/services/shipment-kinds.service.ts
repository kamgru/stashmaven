import {Injectable} from '@angular/core';
import {Observable, of} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";

export class AddShipmentKindRequest {
    constructor(
        public readonly name: string,
        public readonly shortCode: string,
        public readonly direction: 'In' | 'Out'
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class ShipmentKindsService {

    constructor(
        private http: HttpClient
    ) {
    }

    public getDirections(): Observable<('In' | 'Out')[]> {
        return of(['In', 'Out']);
    }

    public addShipmentKind(req: AddShipmentKindRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/shipmentkind`, req);
    }
}
