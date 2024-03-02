import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import {IGetDefaultStockpileIdResponse} from "../components/list-inventory/list-inventory.service";
import {IListStockpilesResponse} from "../../inventory/stockpile-inventory/stockpile-inventory.service";
import {HttpClient} from "@angular/common/http";

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

}
