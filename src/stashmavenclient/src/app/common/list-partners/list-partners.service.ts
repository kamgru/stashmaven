import {Injectable, signal} from '@angular/core';
import {BehaviorSubject, debounceTime, distinctUntilChanged, merge, Observable, Subject, switchMap, tap} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";

class ListPartnersRequest {
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'name';
    public isAscending: boolean = true;
}

export interface IListPartnersResponse {
    items: IPartner[];
    totalCount: number;
}

export interface IPartner {
    partnerId: string;
    legalName: string;
    customIdentifier: string;
    street: string;
    city: string;
    postalCode: string;
    primaryTaxIdentifierType: string;
    primaryTaxIdentifierValue: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListPartnersService {

    private _request = new ListPartnersRequest();
    private _search$ = new Subject<string>();
    private _page$ = new BehaviorSubject<number>(this._request.page);
    private _sortBy$ = new Subject<string>();
    private _pageSize$ = new Subject<number>();
    private _count: number = 0;
    private _partners: IPartner[] = [];

    public totalPages_$ = signal(10);
    public currentIndex_$ = signal(0);
    public pageSize_$ = signal(10);
    public page_$ = signal(1);
    public search_$ = signal('');
    public partners$: Observable<IListPartnersResponse>;
    public selectedPartner$: Subject<IPartner> = new Subject<IPartner>();

    constructor(
        private http: HttpClient,
    ) {
        const search$ = this._search$.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            switchMap(s => {
                this.currentIndex_$.set(0);
                this._request.search = s;
                this._request.page = 1;
                this.page_$.set(1);
                return this.listPartners(this._request);
            }));

        const page$ = this._page$.pipe(
            distinctUntilChanged(),
            switchMap(p => {
                this._request.page = p;
                this.page_$.set(p);
                return this.listPartners(this._request);
            }));

        const sortBy$ = this._sortBy$.pipe(
            distinctUntilChanged(),
            switchMap(s => {
                this._request.sortBy = s;
                this._request.page = 1;
                this.page_$.set(1);
                return this.listPartners(this._request);
            }));

        const pageSize$ = this._pageSize$.pipe(
            distinctUntilChanged(),
            switchMap(s => {
                this.currentIndex_$.set(0);
                this._request.pageSize = s;
                this._request.page = 1;
                this.pageSize_$.set(s);
                this.page_$.set(1);
                return this.listPartners(this._request);
            }));

        this.partners$ = merge(page$, search$, sortBy$, pageSize$)
            .pipe(
                tap(x => {
                    this.totalPages_$.set(Math.ceil(x.totalCount / this._request.pageSize));
                    this._count = x.items.length;
                    this._partners.splice(0, this._partners.length);
                    this._partners.push(...x.items);
                }));
    }

    public listPartners(req: ListPartnersRequest): Observable<IListPartnersResponse> {
        return this.http.get<IListPartnersResponse>(`${environment.apiUrl}/api/v1/partner/list`, {
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
                const partner = this._partners[index];
                this.selectedPartner$.next(partner);
                return true;
            }
        }
        return false;
    }
}
