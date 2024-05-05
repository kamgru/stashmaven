import {Injectable} from '@angular/core';
import * as li from "../list-items";
import {BehaviorSubject, Observable} from 'rxjs';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

export * from "./list-shipment-kinds.service";

export class ListShipmentKindsRequest implements li.IListRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'name';
    public isAscending: boolean = true;
}

export interface IListShipmentKindsResponse extends li.IListResponse<IShipmentKindItem> {
}

export interface IShipmentKindItem {
    shipmentKindId: string;
    name: string;
    shortCode: string;
    direction: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListShipmentKindsService extends li.ListServiceBase<IShipmentKindItem, ListShipmentKindsRequest, IListShipmentKindsResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListShipmentKindsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListShipmentKindsRequest): Observable<IListShipmentKindsResponse> {
        return this.http.get<IListShipmentKindsResponse>(`${environment.apiUrl}/api/v1/shipmentkind/list`, {
            params: {
                page: request.page,
                pageSize: request.pageSize,
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending,
            }
        });
    }
}
