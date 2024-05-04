import {Injectable} from '@angular/core';
import {map, Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {IGetDefaultStockpileIdResponse} from "../components/list-inventory/list-inventory.service";
import {HttpClient} from "@angular/common/http";

export class AddStockpileRequest {
    constructor(
        public name: string,
        public shortCode: string,
        public isDefault: boolean
    ) {
    }
}

export interface IGetStockpileResponse {
    name: string;
    shortCode: string;
    isDefault: boolean;
}

export class UpdateStockpileRequest {
    constructor(
        public name: string,
        public shortCode: string,
        public isDefault: boolean
    ) {
    }
}

export interface IStockpile {
    stockpileId: string;
    name: string;
    shortCode: string;
    isDefault: boolean;
}

export interface IListStockpilesResponse {
    items: IStockpile[];
}

@Injectable({
    providedIn: 'root'
})
export class StockpileService {

    constructor(
        private http: HttpClient,
    ) {
    }

    public listStockpiles(): Observable<IListStockpilesResponse> {
        return this.http.get<IListStockpilesResponse>(`${environment.apiUrl}/api/v1/stockpile/list`);
    }

    public getStockpile(stockpileId: string): Observable<IGetStockpileResponse> {
        return this.http.get<IGetStockpileResponse>(`${environment.apiUrl}/api/v1/stockpile/${stockpileId}`);
    }

    public getDefaultStockpileId(): Observable<IGetDefaultStockpileIdResponse> {
        return this.http.get<IGetDefaultStockpileIdResponse>(`${environment.apiUrl}/api/v1/stockpile/default`);
    }

    public setDefaultStockpileId(stockpileId: number): Observable<any> {
        return this.http.post(`${environment.apiUrl}/api/v1/stockpile/${stockpileId}/default`, {});
    }

    public addStockpile(request: AddStockpileRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/stockpile`, request);
    }

    public updateStockpile(stockpileId: string, request: UpdateStockpileRequest): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/v1/stockpile/${stockpileId}`, request);
    }
}
