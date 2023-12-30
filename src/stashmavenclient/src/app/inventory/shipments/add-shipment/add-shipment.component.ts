import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Shipment} from "../shipment";

@Component({
    selector: 'app-add-shipment',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './add-shipment.component.html',
    styleUrls: ['./add-shipment.component.css']
})
export class AddShipmentComponent {


    _shipment?: Shipment;

    public set shipment(value: Shipment) {
        this._shipment = value;
    }

}
