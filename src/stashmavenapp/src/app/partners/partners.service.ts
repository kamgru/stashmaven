import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

export class ListPartnerRequest {
  page: number = 1
  pageSize: number = 10
  search?: string
  sortBy?: string
  isAscending?: boolean
}

export interface PartnerList {
  partners: Partner[]
  totalCount: number
}

export interface Partner {
  partnerId: string
  legalName: string
  customIdentifier: string
  street: string
  city: string
  postalCode: string
  primaryTaxIdentifierType: string
  primaryTaxIdentifierValue: string
  createdOn: string
  updatedOn: string
}

@Injectable({
  providedIn: 'root'
})
export class PartnersService {

  constructor(private http: HttpClient) {
  }

  listPartners(req: ListPartnerRequest): Observable<PartnerList> {
    return this.http.get<PartnerList>('http://localhost:5253/api/v1/partner/list',
      {
        params: {...req}
      })
  }

}
