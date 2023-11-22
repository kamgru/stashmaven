import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {Partner} from "./partner";

@Injectable({
  providedIn: 'root'
})
export class SelectPartnerService {
  private selectedPartner$ : BehaviorSubject<Partner | null>  = new BehaviorSubject<Partner | null>(null);
  selectedPartner = this.selectedPartner$.asObservable();

  selectPartner(partner: Partner) {
    this.selectedPartner$.next(partner);
  }
}
