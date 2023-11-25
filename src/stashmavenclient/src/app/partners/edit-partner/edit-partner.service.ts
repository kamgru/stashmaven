import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {TaxIdentifier} from "../taxIdentifier";
import {Observable} from "rxjs";
import {PartnerAddress} from "../partnerAddress";

export interface Partner{
    partnerId: string,
    customIdentifier: string,
    legalName: string,
    taxIdentifiers: TaxIdentifier[],
    address: PartnerAddress,
    createdOn: Date,
    updatedOn: Date,
}

export class PatchPartnerRequest{
    constructor(
        public customIdentifier: string | null = null,
        public legalName: string | null = null,
        public taxIdentifiers: TaxIdentifier[] | null = null,
        public address: PartnerAddress | null = null
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class EditPartnerService {

    constructor(
        private http: HttpClient) {
    }

    getPartner(partnerId: string) : Observable<Partner | null> {
        return this.http.get<Partner>('http://localhost:5253/api/v1/partner/' + partnerId);
    }

    patchPartner(partnerId: string, req: PatchPartnerRequest) : Observable<Partner | null> {
        return this.http.patch<Partner>('http://localhost:5253/api/v1/partner/' + partnerId, req);
    }
}
