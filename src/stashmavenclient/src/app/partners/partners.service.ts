import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export interface Partner {
  partnerId: string
  customIdentifier: string
  legalName: string
  primaryTaxIdentifier: string
  city: string
  street: string
  postalCode: string
  createdOn: string
  updatedOn: string
}

export interface ListPartnersResponse {
  partners: Partner[]
  totalCount: number
}

export class ListPartnersRequest {
  page: number = 1
  pageSize: number = 25
  search?: string
  sortBy?: string
  isAscending?: boolean = false

  reset(){
    this.page = 1
    this.pageSize = 25
  }
}

@Injectable({
  providedIn: 'root'
})
export class PartnersService {

  constructor(private http: HttpClient) {
  }

  listPartners(req: ListPartnersRequest): Observable<ListPartnersResponse> {
    return this.http.get<ListPartnersResponse>('http://localhost:5253/api/v1/partner/list', {params: {...req}});
  }

}
