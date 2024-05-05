import * as li from "../list-items";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "../../../../environments/environment";
import {Injectable} from "@angular/core";

export * from "./list-business-identifiers.service";

export class ListBusinessIdentifiersRequest implements li.IListRequest {
    page: number = 1;
    pageSize: number = 10;
    search: string = '';
    sortBy: string = '';
    isAscending: boolean = true;
}

export interface IBusinessIdentifier {
    businessIdentifierId: string;
    shortCode: string;
    name: string;
}

export interface IListBusinessIdentifiersResponse extends li.IListResponse<IBusinessIdentifier> {
}

@Injectable({
    providedIn: 'root'
})
export class ListBusinessIdentifiersService extends li.ListServiceBase<IBusinessIdentifier, ListBusinessIdentifiersRequest, IListBusinessIdentifiersResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListBusinessIdentifiersRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListBusinessIdentifiersRequest): Observable<IListBusinessIdentifiersResponse> {
        return this.http.get<IListBusinessIdentifiersResponse>(`${environment.apiUrl}/api/v1/businessidentifier/list`, {
            params: {
                page: request.page.toString(),
                pageSize: request.pageSize.toString(),
                search: request.search,
                sortBy: request.sortBy,
                isAscending: request.isAscending.toString()
            }
        });
    }
}
