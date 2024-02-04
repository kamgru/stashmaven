import {Injectable, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, debounceTime, distinctUntilChanged, merge, Observable, Subject, switchMap, tap} from "rxjs";
import {environment} from "../../../environments/environment";

class ListCatalogItemsRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'sku';
    public isAscending: boolean = true;
}

export interface IListCatalogItemsResponse {
    items: ICatalogItem[];
    totalCount: number;
}

export interface ICatalogItem {
    catalogItemId: string;
    sku: string;
    name: string;
    unitOfMeasure: string;
    tax: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListCatalogService {
    private _request = new ListCatalogItemsRequest();
    private _search$ = new Subject<string>();
    private _page$ = new BehaviorSubject<number>(this._request.page);
    private _sortBy$ = new Subject<string>();
    private _pageSize$ = new Subject<number>();
    private _count: number = 0;
    private _catalogItems: ICatalogItem[] = [];

    public totalPages_$ = signal(10);
    public currentIndex_$ = signal(0);
    public pageSize_$ = signal(10);
    public page_$ = signal(1);
    public search_$ = signal('');
    public catalogItems$: Observable<IListCatalogItemsResponse>;
    public selectedCatalogItem$: Subject<ICatalogItem> = new Subject<ICatalogItem>();

    constructor(
        private http: HttpClient,
    ) { const search$ = this._search$.pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap(s => {
            this.currentIndex_$.set(0);
            this._request.search = s;
            this._request.page = 1;
            this.page_$.set(1);
            return this.listCatalogItems(this._request);
        }));

        const page$ = this._page$.pipe(
            distinctUntilChanged(),
            switchMap(p => {
                this._request.page = p;
                this.page_$.set(p);
                return this.listCatalogItems(this._request);
            }));

        const sortBy$ = this._sortBy$.pipe(
            distinctUntilChanged(),
            switchMap(s => {
                this._request.sortBy = s;
                this._request.page = 1;
                this.page_$.set(1);
                return this.listCatalogItems(this._request);
            }));

        const pageSize$ = this._pageSize$.pipe(
            distinctUntilChanged(),
            switchMap(s => {
                this.currentIndex_$.set(0);
                this._request.pageSize = s;
                this._request.page = 1;
                this.pageSize_$.set(s);
                this.page_$.set(1);
                return this.listCatalogItems(this._request);
            }));

        this.catalogItems$ = merge(page$, search$, sortBy$, pageSize$)
            .pipe(
                tap(x => {
                    this.totalPages_$.set(Math.ceil(x.totalCount / this._request.pageSize));
                    this._count = x.items.length;
                    this._catalogItems = x.items;
                }));
    }

    public listCatalogItems(req: ListCatalogItemsRequest): Observable<IListCatalogItemsResponse> {
        return this.http.get<IListCatalogItemsResponse>(`${environment.apiUrl}/api/v1/catalogitem/list`, {
            params: {
                page: req.page,
                pageSize: req.pageSize,
                search: req.search,
                sortBy: req.sortBy,
                isAscending: req.isAscending,
            }
        });
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
        this._page$.next(this._page$.getValue() + 1);
        return true;
    }

    tryPrevPage(): boolean {
        if (this._request.page <= 1) {
            return false;
        }
        this._request.page--;
        this._page$.next(this._page$.getValue() - 1);
        return true;
    }

    sortBy(value: string) {
        this._sortBy$.next(value);
    }

    search(value: string) {
        this.search_$.set(value);
        this._search$.next(value);
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
                const catalogItem = this._catalogItems[index];
                this.selectedCatalogItem$.next(catalogItem);
                return true;
            }
        }
        return false;
    }
}
