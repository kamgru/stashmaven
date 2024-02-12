import {Injectable, signal} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {IListRequest, IListResponse, ListServiceBase} from "../ListServiceBase";

export class ListPartnersRequest implements IListRequest{
    public page: number = 1;
    public pageSize: number = 10;
    public search: string = '';
    public sortBy: string = 'name';
    public isAscending: boolean = true;
}

export interface IListPartnersResponse extends IListResponse<IPartner>{
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
export class ListPartnersService extends ListServiceBase<IPartner, ListPartnersRequest, IListPartnersResponse>{

    constructor(
        private http: HttpClient,
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListPartnersRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListPartnersRequest): Observable<IListPartnersResponse> {
        return this.http.get<IListPartnersResponse>(`${environment.apiUrl}/api/v1/partner/list`, {
            params: {
                page: request.page,
                pageSize: request.pageSize,
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending,
            }
        });
    }

}
