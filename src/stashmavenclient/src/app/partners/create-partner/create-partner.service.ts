import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Partner} from "../partner";
import {PartnerAddress} from "../partnerAddress";
import {TaxIdentifier} from "../taxIdentifier";

export class CreatePartnerRequest {
   constructor(
         public customIdentifier: string,
         public legalName: string,
         public taxIdentifiers: TaxIdentifier[],
         public address: PartnerAddress,
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class CreatePartnerService {

    constructor(private http: HttpClient) {
    }

    createPartner(req: CreatePartnerRequest): Observable<Partner> {
        return this.http.post<Partner>('http://localhost:5253/api/v1/partner', req);
    }
}
