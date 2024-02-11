import {Injectable, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {
    BehaviorSubject,
    debounceTime,
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
}

export interface IGetDefaultStockpileIdResponse {
    stockpileId: string;
    name: string;
    shortCode: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListInventoryService {

    public totalPages_$ = signal(10);
    public currentIndex_$ = signal(0);
    public pageSize_$ = signal(10);
    public page_$ = signal(1);
    public search_$ = signal('');
    public selectedInventoryItem$: Subject<IInventoryItem> = new Subject<IInventoryItem>();
    public inventoryItems$: Observable<IListInventoryItemsResponse>;

    private _request$ = new BehaviorSubject(new ListInventoryItemsRequest());

    constructor(
        private http: HttpClient,
    ) {
        const req$ = this._request$.pipe(
            distinctUntilChanged(),
            switchMap(request => {
                return this.listInventoryItems(request)
            }),
        );

        this.inventoryItems$ = merge(req$).pipe(
            tap(response => {
                this.totalPages_$.set(Math.ceil(response.totalCount / this._request$.value.pageSize));
                this.page_$.set(this._request$.value.page);
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
                }
            ));
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
    }

    changePageSize(value: number) {
        value = Math.max(1, value);
        value = Math.min(100, value);
        this.pageSize_$.set(value);
        this._request$.next({...this._request$.value, pageSize: value, page: 1});
    }

    tryNextPage(): boolean {
        if (this.page_$() < this.totalPages_$()) {
            this._request$.next({...this._request$.value, page: this.page_$() + 1})
        }
        return true;
    }

    tryPrevPage(): boolean {
        if (this.page_$() > 1) {
            this._request$.next({...this._request$.value, page: this.page_$() - 1})
        }
        return true;
    }

    sortBy(value: string) {
        this._request$.next({...this._request$.value, sortBy: value});
    }

    search(value: string) {
        this.search_$.set(value);
        this._request$.next({...this._request$.value, search: value, page: 1});
    }

    tryHandleKey(event: KeyboardEvent): boolean {
        switch (event.key) {
            case 'ArrowDown': {
                return true;
            }
            case 'ArrowUp': {
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
                return true;
            }
        }
        return false;
    }
}
