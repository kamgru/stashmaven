import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Brand} from "../brand";
import {Observable} from "rxjs";
import {ISuperTableListService, SuperTableListRequest, SuperTableListResponse} from "../../common/super-table/super-table-list-service";

export class ListBrandsRequest {
    page: number = 1
    pageSize: number = 25
    search?: string
    isAscending?: boolean = true
}

export interface ListBrandsResponse {
    brands: Brand[]
    totalCount: number
}


@Injectable({
    providedIn: 'root'
})
export class ListBrandsService implements ISuperTableListService<Brand> {

    constructor(
        private http: HttpClient,
    ) {
    }

    list(req: SuperTableListRequest): Observable<SuperTableListResponse<Brand>> {
        console.log(req)
        console.log(req.toHttpParams())
        return this.http.get<SuperTableListResponse<Brand>>('http://localhost:5253/api/v1/brand/list', {params: req.toHttpParams()})
    }

    listBrands(req: ListBrandsRequest): Observable<ListBrandsResponse> {
        return this.http.get<ListBrandsResponse>('http://localhost:5253/api/v1/brand/list', {params: {...req}})
    }

}
