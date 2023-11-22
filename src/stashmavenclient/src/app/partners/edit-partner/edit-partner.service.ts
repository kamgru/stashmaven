import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {TaxIdentifier} from "../taxIdentifier";
import {Observable} from "rxjs";

export interface Partner{
    partnerId: string,
    customIdentifier: string,
    legalName: string,
    taxIdentifiers: TaxIdentifier[],
    street: string,
    streetAdditional?: string,
    city: string,
    state?: string,
    postalCode: string,
    countryCode: string,
    createdOn: Date,
    updatedOn: Date,
}

@Injectable({
    providedIn: 'root'
})
export class EditPartnerService {

    constructor(
        private http: HttpClient) {
    }

    getPartner(partnerId: string) : Observable<Partner> {
        return this.http.get<Partner>('http://localhost:5253/api/v1/partner/' + partnerId);
    }
}
