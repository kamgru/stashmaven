import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {
    IShipmentEditDetails,
    IShipmentPartnerEditDetails,
    ShipmentService
} from "../../../common/services/shipment.service";
import {JsonPipe} from "@angular/common";
import {ListPartnersComponent} from "../../../common/list-partners/list-partners.component";
import {IPartner} from "../../../common/list-partners/list-partners.service";
import {AddRecordToShipmentRequest, EditShipmentService} from "./edit-shipment.service";
import {ListInventoryComponent} from "../../../common/list-inventory/list-inventory.component";
import {IInventoryItem} from "../../../common/list-inventory/list-inventory.service";
import {PartnerDetailsComponent} from "./partner-details/partner-details.component";

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        JsonPipe,
        ListPartnersComponent,
        ListInventoryComponent,
        PartnerDetailsComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styleUrl: './edit-shipment.component.css'
})
export class EditShipmentComponent implements OnInit {

    private shipmentId: string | null = null;

    public shipment?: IShipmentEditDetails;
    public uiState: 'view' | 'edit-partner' | 'add-item' = 'view';

    constructor(
        private route: ActivatedRoute,
        private shipmentService: ShipmentService,
        private editShipmentService: EditShipmentService
    ) {
    }

    ngOnInit(): void {
        this.shipmentId = this.route.snapshot.paramMap.get('shipmentId');
        if (this.shipmentId) {
            this.shipmentService.getShipment(this.shipmentId)
                .subscribe(response => {
                    this.shipment = response;
                });
        }
    }

    handlePartnerConfirmed($event: IPartner) {
        if (!this.shipmentId) {
            throw new Error('shipmentId is not set');
        }

        this.editShipmentService.addPartnerToShipment(this.shipmentId, $event.partnerId)
            .subscribe(_ => {
                this.shipment!.partner = <IShipmentPartnerEditDetails>{
                    partnerId: $event.partnerId,
                    legalName: $event.legalName,
                    customIdentifier: $event.customIdentifier,
                    address: $event.street + ', ' + $event.city + ', ' + $event.postalCode
                }
            });
    }

    handleInventoryItemConfirmed($event: IInventoryItem) {
        this.editShipmentService.addRecordToShipment(new AddRecordToShipmentRequest(
            this.shipmentId!,
            $event.inventoryItemId,
            1,
            123,
        ))
            .subscribe(_ => {
                this.shipment!.records.push({
                    inventoryItemId: $event.inventoryItemId,
                    quantity: 1,
                    unitPrice: 123,
                    sku: $event.sku,
                    name: $event.name,
                    taxRate: 23
                });
            });
    }
}
