import {BehaviorSubject, distinctUntilChanged, map, Observable, Subject, switchMap, tap} from "rxjs";
import {Cursor} from "./Cursor";
import {computed} from "@angular/core";

export interface IListResponse<T> {
    items: T[];
    totalCount: number;
    totalPages: number;
    currentPage: number;
    search: string;
    sortBy: string;
    isAscending: boolean;
    pageSize: number;
}

export interface IListRequest {
    page: number;
    pageSize: number;
    search: string;
    sortBy: string;
    isAscending: boolean;
}

export abstract class ListServiceBase<TItem, TListRequest extends IListRequest, TListResponse extends IListResponse<any>> {
    protected _request$!: BehaviorSubject<TListRequest>;
    protected _response!: IListResponse<TItem>;
    protected _cursor = new Cursor();

    public currentIndex_$ = computed(() => this._cursor.index());
    public items$!: Observable<TListResponse>;
    public selectedItem$ = new Subject<TItem | null>()
    public confirmedItem$ = new Subject<TItem>();

    protected abstract listItems(request: TListRequest): Observable<TListResponse>;

    protected bootstrap() {

        this.items$ = this._request$.pipe(
            distinctUntilChanged((prev, curr) => {
                if ((<any>curr).force) {
                    return false;
                }
                return JSON.stringify(prev) === JSON.stringify(curr);
            }),
            switchMap(request => this.listItems(request)),
            map(response => ({
                ...response,
                pageSize: this._request$.value.pageSize,
                search: this._request$.value.search,
                sortBy: this._request$.value.sortBy,
                isAscending: this._request$.value.isAscending,
                currentPage: this._request$.value.page,
                totalPages: Math.ceil(response.totalCount / this._request$.value.pageSize)
            })),
            tap(response => {
                this._response = response;
                if (this._cursor.tryReset(response.items.length)){
                    this.selectedItem$.next(response.items[this._cursor.index()]);
                }
            })
        );
    }

    reload() {
        this._request$.next({...this._request$.value, force: true});
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
                return this._cursor.tryMoveDown() || this.tryNextPage();
            }
            case 'ArrowUp': {
                return this._cursor.tryMoveUp() || this.tryPrevPage();
            }
            case 'PageDown': {
                return this.tryNextPage();
            }
            case 'PageUp': {
                return this.tryPrevPage();
            }
            case 'Enter': {
                this.confirmedItem$.next(this._response.items[this._cursor.index()]);
                return true;
            }
        }
        return false;
    }

    select(item: TItem) {
        const index = this._response.items.findIndex(x => JSON.stringify(x) === JSON.stringify(item));
        if (this._cursor.trySetIndex(index)) {
            this.selectedItem$.next(item);
        }
    }

    confirm(item: TItem) {
        this.confirmedItem$.next(item);
    }
}