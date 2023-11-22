import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Partner} from "../partner";

export class ListPartnersRequest {
    page: number = 1
    pageSize: number = 25
    search?: string
    sortBy?: string
    isAscending?: boolean = false

    reset() {
        this.page = 1
        this.pageSize = 25
    }
}

export interface ListPartnersResponse {
    partners: Partner[]
    totalCount: number
}

@Injectable({
    providedIn: 'root'
})
export class ListPartnersService {


    constructor(private http: HttpClient) {
    }

    listPartners(req: ListPartnersRequest): Observable<ListPartnersResponse> {
        return this.http.get<ListPartnersResponse>('http://localhost:5253/api/v1/partner/list', {params: {...req}});
    }
}
