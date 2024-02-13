import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "../../../environments/environment";
import * as li from "../components/list-items";

export * from "./list-shipments.service";

export class ListShipmentsRequest implements li.IListRequest{
    page: number = 1;
    pageSize: number = 10;
    search: string = '';
    sortBy: string = '';
    isAscending: boolean = true;
    stockpileId: string = '';
}

export interface IShipment {
    shipmentId: string;
    kindShortCode: string;
    sequenceNumber: string;
    partnerIdentifier: string;
    totalMoney: number;
    acceptanceStatus: string;
    createdOn: string;
}

export interface IListShipmentsResponse extends li.IListResponse<IShipment> {
    stockpileId: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListShipmentsService extends li.ListServiceBase<IShipment, ListShipmentsRequest, IListShipmentsResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListShipmentsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListShipmentsRequest): Observable<IListShipmentsResponse> {
        return this.http.get<IListShipmentsResponse>(`${environment.apiUrl}/api/v1/shipment/list`, {
            params: {
                stockpileId: request.stockpileId,
                page: request.page.toString(),
                pageSize: request.pageSize.toString()
            }
        });
    }

    changeStockpile(stockpileId: string) {
        this._request$.next({...this._request$.value, stockpileId, page: 1});
    }
}
