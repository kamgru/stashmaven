import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {catchError} from "rxjs";

export class AddPartnerAddress {
    constructor(
        public street: string,
        public streetAdditional: string | null,
        public city: string,
        public postalCode: string,
        public countryCode: string
    ) {
    }
}

export class AddPartnerBusinessIdentifier{
    constructor(
        public type: string,
        public value: string
    ) {
    }
}

export class AddPartnerRequest{
   constructor(
       public customIdentifier: string,
       public legalName: string,
       public address: AddPartnerAddress,
       public businessIdentifiers: AddPartnerBusinessIdentifier[]
   ) {
   }
}

@Injectable({
    providedIn: 'root'
})
export class PartnerService {

    constructor(
        private http: HttpClient
    ) {
    }

    public addPartner(req: AddPartnerRequest) {
        return this.http.post(`${environment.apiUrl}/api/v1/partner`, req);
    }

    deletePartner(partnerId: string) {
        return this.http.delete(`${environment.apiUrl}/api/v1/partner/${partnerId}`);
    }
}
