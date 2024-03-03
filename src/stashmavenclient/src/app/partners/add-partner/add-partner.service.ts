import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifier} from "../taxIdentifier";
import {environment} from "../../../environments/environment";

export class AddPartnerRequest {
   constructor(
         public customIdentifier: string,
         public legalName: string,
         public taxIdentifiers: TaxIdentifier[],
         public address: PartnerAddress,
         public isRetail: boolean,
    ) {
    }
}

export interface IAddPartnerResponse {
    partnerId: string;
}

@Injectable({
    providedIn: 'root'
})
export class AddPartnerService {

    constructor(private http: HttpClient) {
    }

    addPartner(req: AddPartnerRequest): Observable<IAddPartnerResponse> {
        return this.http.post<IAddPartnerResponse>(`${environment.apiUrl}/api/v1/partner`, req);

    }
}
