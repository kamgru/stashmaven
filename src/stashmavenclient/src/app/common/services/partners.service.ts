import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class ListPartnersRequest {
    page: number = 1
    pageSize: number = 25
    search?: string
    sortBy?: string
    isAscending: boolean = false

    constructor(
        page: number = 1,
        pageSize: number = 25,
        search?: string,
        sortBy?: string,
        isAscending: boolean = false
    ) {
        this.page = page;
        this.pageSize = pageSize;
        this.search = search;
        this.sortBy = sortBy;
        this.isAscending = isAscending;
    }
}

export interface IListPartnersResponse {
    items: IPartner[],
    totalCount: number
}

export interface IPartner {
    partnerId: string,
    legalName: string,
    customIdentifier: string,
    street: string,
    city: string,
    postalCode: string,
    primaryTaxIdentifierType: string,
    primaryTaxIdentifierValue: string,
}

@Injectable({
    providedIn: 'root'
})
export class PartnersService {

    constructor(
        private http: HttpClient
    ) {
    }

    listPartners(req: ListPartnersRequest): Observable<IListPartnersResponse> {
        return this.http.get<IListPartnersResponse>('http://localhost:5253/api/v1/partner/list',
            {
                params: {
                    page: req.page.toString(),
                    pageSize: req.pageSize.toString(),
                    sortBy: req.sortBy || '',
                    isAscending: req.isAscending.toString(),
                    search: req.search || '',
                }});
    }
}
