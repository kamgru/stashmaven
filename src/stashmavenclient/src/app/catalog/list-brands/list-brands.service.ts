import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Brand} from "../brand";
import {Observable} from "rxjs";

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
export class ListBrandsService {

    constructor(
        private http: HttpClient,
    ) {
    }

    listBrands(req: ListBrandsRequest): Observable<ListBrandsResponse> {
        return this.http.get<ListBrandsResponse>('http://localhost:5253/api/v1/brand/list', {params: {...req}})
    }

}
