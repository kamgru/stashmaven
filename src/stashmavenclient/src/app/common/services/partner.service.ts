import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class PartnerService {

    constructor(
        private http: HttpClient
    ) {
    }

    deletePartner(partnerId: string) {
        return this.http.delete(`${environment.apiUrl}/api/v1/partner/${partnerId}`)
    }
}
