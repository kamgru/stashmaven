import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Shipment, ShipmentPartner} from "../shipment";
import {
    SelectShipmentPartnerComponent
} from "./select-shipment-partner/select-shipment-partner/select-shipment-partner.component";
import {IPartner} from "../../../common/services/partners.service";
import {AddPartnerToShipmentRequest, ShipmentsService} from "../shipments.service";

enum UiState {
    SelectPartner = 'select-partner',
    View = 'view'
}

@Component({
    selector: 'app-add-shipment',
    standalone: true,
    imports: [CommonModule, SelectShipmentPartnerComponent],
    templateUrl: './add-shipment.component.html',
    styleUrls: ['./add-shipment.component.css']
})
export class AddShipmentComponent {


    _shipment?: Shipment;

    uiState = UiState.SelectPartner;

    constructor(
        private shipmentsService: ShipmentsService
    ) {
    }

    public set shipment(value: Shipment) {
        this._shipment = value;
    }

    handlePartnerSelected($event: IPartner) {
        if (!this._shipment) {
            throw new Error('Shipment is not set');
        }

        const req = new AddPartnerToShipmentRequest($event.partnerId);
        this.shipmentsService.addPartnerToShipment(this._shipment.shipmentId, req)
            .subscribe(data => {
                const address = `${$event.street}, ${$event.postalCode} ${$event.city}`;
                this._shipment!.partner = new ShipmentPartner(
                    $event.partnerId,
                    $event.customIdentifier,
                    $event.legalName,
                    address,
                )
                this.uiState = UiState.View;
            });
    }
}
