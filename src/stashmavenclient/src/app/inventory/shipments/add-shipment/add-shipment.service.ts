import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";

export class AddShipmentRequest {

}

Injectable({
  providedIn: 'root'
})
export class AddShipmentService {

  constructor(
      private http: HttpClient,
  ) { }


}
