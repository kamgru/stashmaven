import {Injectable, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {distinctUntilChanged, merge, Observable, Subject, switchMap, tap} from "rxjs";
import {environment} from "../../../environments/environment";
import {IGetDefaultStockpileIdResponse} from "../list-inventory/list-inventory.service";

class ListShipmentsRequest {
    public stockpileId: string = '';
    public page: number = 1;
    public pageSize: number = 10;
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

export interface IListShipmentsResponse {
    items: IShipment[];
    totalCount: number;
}

@Injectable({
    providedIn: 'root'
})
export class ListShipmentsService {

    private _request = new ListShipmentsRequest();
    private _page$ = new Subject<number>();
    private _pageSize$ = new Subject<number>();
    private _stockpileId$ = new Subject<string>();
    private _stockpileId_$ = signal('');
    private _count: number = 0;
    private _shipments: IShipment[] = [];

    public totalPages_$ = signal(0);
    public currentIndex_$ = signal(0);
    public pageSize_$ = signal(10);
    public page_$ = signal(1);

    public shipments$: Observable<IListShipmentsResponse>;
    public selectedShipment$: Subject<IShipment> = new Subject<IShipment>();

    constructor(
        private http: HttpClient
    ) {
        this.getDefaultStockpileId()
            .subscribe(x => {
                this._stockpileId_$.set(x.stockpileId);
                this._stockpileId$.next(x.stockpileId)
            });

        const stockpileId$ = this._stockpileId$.pipe(
            switchMap(stockpileId => {
                this._request.page = 1;
                this._request.stockpileId = stockpileId;
                this.page_$.set(1);
                return this.listShipments(this._request);
            }));

        const page$ = this._page$.pipe(
            distinctUntilChanged(),
            switchMap(page => {
                this._request.page = page;
                this.page_$.set(page);
                return this.listShipments(this._request);
            }));

        const pageSize$ = this._pageSize$.pipe(
            distinctUntilChanged(),
            switchMap(pageSize => {
                this._request.pageSize = pageSize;
                this.pageSize_$.set(pageSize);
                return this.listShipments(this._request);
            }));

        this.shipments$ = merge(stockpileId$, page$, pageSize$)
            .pipe(
                tap(x => {
                    this._count = x.totalCount;
                    this._shipments.splice(0, this._shipments.length);
                    this._shipments.push(...x.items);
                    this.totalPages_$.set(Math.ceil(this._count / this._request.pageSize));
                })
            )
    }

    private listShipments(req: ListShipmentsRequest): Observable<IListShipmentsResponse> {
        return this.http.get<IListShipmentsResponse>(`${environment.apiUrl}/api/v1/shipment/list`, {
            params: {
                stockpileId: req.stockpileId,
                page: req.page.toString(),
                pageSize: req.pageSize.toString()
            }
        });
    }

    private getDefaultStockpileId(): Observable<IGetDefaultStockpileIdResponse> {
        return this.http.get<IGetDefaultStockpileIdResponse>(`${environment.apiUrl}/api/v1/stockpile/default`);
    }

    changeStockpile(stockpileId: string) {
        this._stockpileId$.next(stockpileId);
    }

    changePageSize(value: number) {
        const pageSize = Math.min(Math.max(value, 10), 100);
        this._pageSize$.next(pageSize);
    }

    tryNextPage(): boolean {
        if (this._request.page >= this.totalPages_$()) {
            return false;
        }
        this._request.page++;
        this._page$.next(this.page_$() + 1);
        return true;
    }

    tryPrevPage(): boolean {
        if (this._request.page <= 1) {
            return false;
        }
        this._request.page--;
        this._page$.next(this.page_$() - 1);
        return true;
    }

    tryHandleKey(event: KeyboardEvent): boolean {
        switch (event.key) {
            case 'ArrowDown': {
                this.currentIndex_$.set(this.currentIndex_$() + 1);
                if (this.currentIndex_$() > this._count - 1) {
                    if (this.tryNextPage()) {
                        this.currentIndex_$.set(0);
                    } else {
                        this.currentIndex_$.set(this._count - 1);
                    }
                }
                return true;
            }
            case 'ArrowUp': {
                this.currentIndex_$.set(this.currentIndex_$() - 1);
                if (this.currentIndex_$() < 0) {
                    if (this.page_$() > 1) {
                        if (this.tryPrevPage()) {
                            this.currentIndex_$.set(this.pageSize_$() - 1);
                        }
                    } else {
                        this.currentIndex_$.set(0);
                    }
                }
                return true;
            }
            case 'PageDown': {
                this.tryNextPage();
                return true;
            }
            case 'PageUp': {
                this.tryPrevPage();
                return true;
            }
            case 'Enter': {
                const index = this.currentIndex_$();
                const shipment = this._shipments[index];
                this.selectedShipment$.next(shipment);
                return true;
            }
        }
        return false;
    }
}
