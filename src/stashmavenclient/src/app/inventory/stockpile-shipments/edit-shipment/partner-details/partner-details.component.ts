import {Component, EventEmitter, Input, Output} from '@angular/core';
import {IShipmentPartner} from "../../../../common/services/shipment.service";

@Component({
    selector: 'app-partner-details',
    standalone: true,
    imports: [],
    templateUrl: './partner-details.component.html',
    styleUrl: './partner-details.component.css'
})
export class PartnerDetailsComponent {

    @Input()
    public partner?: IShipmentPartner;

    @Output()
    public OnPartnerEdit = new EventEmitter<void>();
}
