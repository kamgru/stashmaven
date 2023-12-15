import {Observable} from "rxjs";

export class SuperGridListRequest {
    page: number;
    pageSize: number;
    search?: string;
    sortBy?: string;
    isAscending: boolean = true;

    constructor(
        page: number,
        pageSize: number,
        search?: string,
        sortBy?: string,
        isAscending: boolean = true) {
        this.page = page;
        this.pageSize = pageSize;
        this.search = search;
        this.sortBy = sortBy;
        this.isAscending = isAscending;
    }

    toHttpParams(): any {
        return {
            page: this.page,
            pageSize: this.pageSize,
            search: this.search ?? '',
            sortBy: this.sortBy ?? '',
            isAscending: this.isAscending,
        }
    }

    nextPage() {
        this.page += 1;
    }

    firstPage() {
        this.page = 1;
    }

    clearFilters() {
        this.search = '';
        this.sortBy = '';
        this.isAscending = true;
    }
}

export class SuperGridListResponse<T> {
    constructor(
        public items: T[],
        public totalCount: number) {
    }
}

export interface ISuperGridListService<TData> {
    list(req: SuperGridListRequest): Observable<SuperGridListResponse<TData>>;
}
