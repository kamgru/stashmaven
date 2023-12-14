import {Observable} from "rxjs";

export class SuperTableListRequest {
    constructor(
        public page: number,
        public pageSize: number,
        public search?: string,
        public sortBy?: string,
        public isAscending: boolean = true) {
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

    nextPage(){
        this.page += 1;
    }

    firstPage(){
        this.page = 1;
    }

    clearFilters(){
        this.search = '';
        this.sortBy = '';
        this.isAscending = true;
    }
}

export class SuperTableListResponse<T> {
    constructor(
        public items: T[],
        public totalCount: number) {
    }
}

export interface ISuperTableListService<TData> {
    list(req: SuperTableListRequest): Observable<SuperTableListResponse<TData>>;
}