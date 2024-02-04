import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {Observable} from "rxjs";
import {IGetDefaultStockpileIdResponse} from "../../common/list-inventory/list-inventory.service";

export interface IStockpile {
    stockpileId: string;
    name: string;
    shortCode: string;
}

export interface IListStockpilesResponse {
    items: IStockpile[];
    totalCount: number;
}

@Injectable({
    providedIn: 'root'
})
export class StockpileInventoryService {
    constructor(
        private http: HttpClient,
    ) {
    }

    public listStockpiles(): Observable<IListStockpilesResponse> {
        return this.http.get<IListStockpilesResponse>(`${environment.apiUrl}/api/v1/stockpile/list`, {
            params: {
                page: '1',
                pageSize: '25',
            }
        });
    }

    public getDefaultStockpileId(): Observable<IGetDefaultStockpileIdResponse> {
        return this.http.get<IGetDefaultStockpileIdResponse>(`${environment.apiUrl}/api/v1/stockpile/default`);
    }
}
