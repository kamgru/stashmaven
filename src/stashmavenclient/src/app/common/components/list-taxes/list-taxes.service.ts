import * as li from "../list-items";
import {HttpClient} from "@angular/common/http";
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "../../../../environments/environment";
import {Injectable} from "@angular/core";

export * from "./list-taxes.service";

export class ListTaxDefinitionsRequest implements li.IListRequest {
    page: number = 1;
    pageSize: number = 10;
    search: string = '';
    sortBy: string = '';
    isAscending: boolean = true;
}

export interface ITaxDefinition {
    taxDefinitionId: string;
    name: string;
    rate: number;
}

export interface IListTaxDefinitionsResponse extends li.IListResponse<ITaxDefinition> {
    taxDefinitionId: string;
}

@Injectable({
    providedIn: 'root'
})
export class ListTaxDefinitionsService extends li.ListServiceBase<ITaxDefinition, ListTaxDefinitionsRequest, IListTaxDefinitionsResponse> {

    constructor(
        private http: HttpClient
    ) {
        super();
        this._request$ = new BehaviorSubject(new ListTaxDefinitionsRequest());
        this.bootstrap();
    }

    protected override listItems(request: ListTaxDefinitionsRequest): Observable<IListTaxDefinitionsResponse> {
        return this.http.get<IListTaxDefinitionsResponse>(`${environment.apiUrl}/api/v1/taxDefinition/list`, {
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
