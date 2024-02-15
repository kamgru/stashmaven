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
import {AddRecordToShipmentRequest, EditShipmentService, IInventoryItemDetails} from "./edit-shipment.service";
import {ListInventoryComponent} from "../../../common/list-inventory/list-inventory.component";
import {IInventoryItem} from "../../../common/list-inventory/list-inventory.service";
import {PartnerDetailsComponent} from "./partner-details/partner-details.component";
import {AddRecordComponent, RecordAdded} from "./add-record/add-record.component";

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        JsonPipe,
        ListPartnersComponent,
        ListInventoryComponent,
        PartnerDetailsComponent,
        AddRecordComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styleUrl: './edit-shipment.component.css'
})
export class EditShipmentComponent implements OnInit {

    private _shipmentId: string | null = null;

    public shipment?: IShipmentEditDetails;
    public uiState: 'view' | 'edit-partner' | 'select-item' | 'add-record' = 'view';
    public inventoryItem: IInventoryItemDetails | null = null;

    constructor(
        private route: ActivatedRoute,
        private shipmentService: ShipmentService,
        private editShipmentService: EditShipmentService
    ) {
    }

    ngOnInit(): void {
        this._shipmentId = this.route.snapshot.paramMap.get('shipmentId');
        if (this._shipmentId) {
            this.shipmentService.getShipment(this._shipmentId)
                .subscribe(response => {
                    this.shipment = response;
                });
        }
    }

    handlePartnerConfirmed($event: IPartner) {
        if (!this._shipmentId) {
            throw new Error('shipmentId is not set');
        }

        this.editShipmentService.addPartnerToShipment(this._shipmentId, $event.partnerId)
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
        this.editShipmentService.getInventoryItem($event.inventoryItemId)
            .subscribe(response => {
                this.inventoryItem = response;
                this.uiState = 'add-record';
            })
    }

    handleRecordAdded($event: RecordAdded) {
        this.editShipmentService.addRecordToShipment(new AddRecordToShipmentRequest(
            this._shipmentId!,
            $event.inventoryItem.inventoryItemId,
            $event.quantity,
            $event.unitPrice,
        ))
            .subscribe(_ => {
                this.shipment!.records.push({
                    inventoryItemId: $event.inventoryItem.inventoryItemId,
                    quantity: $event.quantity,
                    unitPrice: $event.unitPrice,
                    sku: $event.inventoryItem.sku,
                    name: $event.inventoryItem.name,
                    taxRate: this.shipment!.kind.direction === 'inbound'
                        ? $event.inventoryItem.buyTaxRate
                        : $event.inventoryItem.sellTaxRate
                });

                this.uiState = 'view';
            });

    }
}
