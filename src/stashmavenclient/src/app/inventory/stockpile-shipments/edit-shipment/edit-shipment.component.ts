import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {
    AddRecordToShipmentRequest,
    IGetShipmentResponse,
    IShipmentPartner,
    ShipmentService
} from "../../../common/services/shipment.service";
import {JsonPipe} from "@angular/common";
import {ListPartnersComponent, IPartner} from "../../../common/components/list-partners";
import {ListInventoryComponent} from "../../../common/components/list-inventory/list-inventory.component";
import {IInventoryItem} from "../../../common/components/list-inventory/list-inventory.service";
import {PartnerDetailsComponent} from "./partner-details/partner-details.component";
import {AddRecordComponent, RecordAdded} from "./add-record/add-record.component";
import {IInventoryItemDetails, InventoryItemService} from "../../../common/services/inventory-item.service";
import {ListRecordsComponent} from "./list-records/list-records.component";

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        JsonPipe,
        ListPartnersComponent,
        ListInventoryComponent,
        PartnerDetailsComponent,
        AddRecordComponent,
        ListRecordsComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styleUrl: './edit-shipment.component.css'
})
export class EditShipmentComponent implements OnInit {

    private _shipmentId: string | null = null;

    public shipment?: IGetShipmentResponse;
    public uiState: 'view' | 'edit-partner' | 'select-item' | 'add-record' = 'view';
    public inventoryItem: IInventoryItemDetails | null = null;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private shipmentService: ShipmentService,
        private inventoryItemService: InventoryItemService
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

        this.shipmentService.addPartnerToShipment(this._shipmentId, $event.partnerId)
            .subscribe(_ => {
                this.shipment!.partner = <IShipmentPartner>{
                    partnerId: $event.partnerId,
                    legalName: $event.legalName,
                    customIdentifier: $event.customIdentifier,
                    address: $event.street + ', ' + $event.city + ', ' + $event.postalCode
                }
            });
    }

    handleInventoryItemConfirmed($event: IInventoryItem) {
        this.inventoryItemService.getInventoryItem($event.inventoryItemId)
            .subscribe(response => {
                this.inventoryItem = response;
                this.uiState = 'add-record';
            })
    }

    handleRecordAdded($event: RecordAdded) {
        this.shipmentService.addRecordToShipment(this._shipmentId!, new AddRecordToShipmentRequest(
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

    handleAccept() {
        if (!this._shipmentId) {
            throw new Error('shipmentId is not set');
        }

        this.shipmentService.acceptShipment(this._shipmentId)
            .subscribe(_ => {
                this.router.navigate(['/shipments']);
            });
    }
}
