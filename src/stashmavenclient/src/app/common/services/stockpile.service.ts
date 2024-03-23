import {Injectable} from '@angular/core';
import {map, Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {IGetDefaultStockpileIdResponse} from "../components/list-inventory/list-inventory.service";
import {HttpClient} from "@angular/common/http";

export class AddStockpileRequest {
    constructor(
        public name: string,
        public shortCode: string,
    ) {
    }
}

export interface IStockpile {
    stockpileId: string;
    name: string;
    shortCode: string;
}

export interface IListStockpilesResponse {
    items: IStockpile[];
}

interface IAddStockpileResponse {
    stockpileId: string;
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

    public getDefaultStockpileId(): Observable<IGetDefaultStockpileIdResponse> {
        return this.http.get<IGetDefaultStockpileIdResponse>(`${environment.apiUrl}/api/v1/stockpile/default`);
    }

    public setDefaultStockpileId(stockpileId: number): Observable<any> {
        return this.http.post(`${environment.apiUrl}/api/v1/stockpile/${stockpileId}/default`, {});
    }

    public addStockpile(request: AddStockpileRequest): Observable<string> {
        return this.http.post<IAddStockpileResponse>(`${environment.apiUrl}/api/v1/stockpile`, request)
            .pipe(
                map(x => x.stockpileId)
            )
    }
}
