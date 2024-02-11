import {computed, Injectable, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {
    BehaviorSubject,
    distinctUntilChanged, map, merge, Observable, of,
    Subject,
    switchMap,
    tap
} from "rxjs";
import {environment} from "../../../environments/environment";

class ListInventoryItemsRequest {
    public stockpileId: string = '';
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IInventoryItem {
    inventoryItemId: string;
    sku: string;
    name: string;
    quantity: number;
    unitOfMeasure: string;
    lastPurchasePrice: number;
}

export interface IListInventoryItemsResponse {
    items: IInventoryItem[];
    totalCount: number;
    totalPages: number;
    currentPage: number;
    search: string;
    sortBy: string;
    isAscending: boolean;
    pageSize: number;
    stockpileId: string;
}

export interface IGetDefaultStockpileIdResponse {
    stockpileId: string;
    name: string;
    shortCode: string;
}

class Cursor {
    public index: number = 0;
    public direction: 'up' | 'down' = 'down';
    public page: number = 1;

}

@Injectable({
    providedIn: 'root'
})
export class ListInventoryService {

    public currentIndex_$ = computed(() => this._cursor.index);
    public selectedInventoryItem$: Subject<IInventoryItem> = new Subject<IInventoryItem>();
    public inventoryItems$: Observable<IListInventoryItemsResponse>;

    private _request$ = new BehaviorSubject(new ListInventoryItemsRequest());
    private _response: IListInventoryItemsResponse = <IListInventoryItemsResponse>{};
    private _cursor = new Cursor();


    constructor(
        private http: HttpClient,
    ) {
        this.inventoryItems$ = this._request$.pipe(
            distinctUntilChanged((x, y) => {
                return JSON.stringify(x) === JSON.stringify(y);

            }),
            switchMap(request => {
                return this.listInventoryItems(request)
            }),
            map(response => {
                return {
                    ...response,
                    pageSize: this._request$.value.pageSize,
                    search: this._request$.value.search,
                    sortBy: this._request$.value.sortBy,
                    isAscending: this._request$.value.isAscending,
                    currentPage: this._request$.value.page,
                    totalPages: Math.ceil(response.totalCount / this._request$.value.pageSize)
                };
            }),
            tap(x => {
                this._response = x;
                if (this._cursor.direction === 'down') {
                    this._cursor.index = 0;
                }
                else {
                    this._cursor.index = this._response.items.length - 1;
                }
            })
        );
    }

    private listInventoryItems(request: ListInventoryItemsRequest): Observable<IListInventoryItemsResponse> {
        return this.http.get<IListInventoryItemsResponse>(`${environment.apiUrl}/api/v1/inventoryitems/list`, {
            params: {
                stockpileId: request.stockpileId,
                page: request.page.toString(),
                pageSize: request.pageSize.toString(),
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending.toString()
            }
        });
    }

    changeStockpile(stockpileId: string) {
        this._request$.next({...this._request$.value, stockpileId, page: 1});
    }

    changePageSize(value: number) {
        value = Math.max(1, value);
        value = Math.min(100, value);
        this._request$.next({...this._request$.value, pageSize: value, page: 1});
    }

    tryNextPage(): boolean {
        if (this._response.currentPage < this._response.totalPages) {
            this._request$.next({...this._request$.value, page: this._response.currentPage + 1})
            return true;
        }
        return false;
    }

    tryPrevPage(): boolean {
        if (this._response.currentPage > 1) {
            this._request$.next({...this._request$.value, page: this._response.currentPage - 1})
            return true;
        }
        return false;
    }

    sortBy(value: string) {
        this._request$.next({...this._request$.value, sortBy: value});
    }

    search(value: string) {
        this._request$.next({...this._request$.value, search: value, page: 1});
    }

    tryHandleKey(event: KeyboardEvent): boolean {
        switch (event.key) {
            case 'ArrowDown': {
                this._cursor.direction = 'down';

                if (this._cursor.index < this._response.items.length - 1) {
                    this._cursor.index++;
                    return true;
                }
                return this.tryNextPage();
            }
            case 'ArrowUp': {
                if (this.currentIndex_$() > 0) {
                    return true;
                } else if (this.tryPrevPage()) {
                    return true;
                }
                return false;
            }
            case 'PageDown': {
                return this.tryNextPage();
            }
            case 'PageUp': {
                return this.tryPrevPage();
            }
            case 'Enter': {
                return true;
            }
        }
        return false;
    }
}
