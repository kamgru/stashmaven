import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {IGetShipmentResponse, ShipmentService} from "../../../common/services/shipment.service";
import {FormArray, FormBuilder, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {BehaviorSubject, Observable, switchMap} from "rxjs";
import {AsyncPipe, formatDate, JsonPipe} from "@angular/common";
import {ListInventoryComponent} from "../../../common/components/list-inventory/list-inventory.component";
import {IInventoryItem} from "../../../common/components/list-inventory/list-inventory.service";
import {IStockpileListItem} from "../../../common/IStockpileListItem";
import {AddedRecord, AddRecordComponent} from "./add-record/add-record.component";
import {IInventoryItemDetails, InventoryItemService} from "../../../common/services/inventory-item.service";
import {IPartner, ListPartnersComponent} from "../../../common/components/list-partners";

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        ListInventoryComponent,
        AsyncPipe,
        JsonPipe,
        AddRecordComponent,
        ListPartnersComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styleUrl: './edit-shipment.component.css'
})
export class EditShipmentComponent implements OnInit {

    public uiState: 'edit' | 'list-inventory-items' | 'add-record' | 'list-partners' = 'edit';

    public shipment$: Observable<IGetShipmentResponse> = this.route.params
        .pipe(switchMap(params => this.shipmentService.getShipment(params['shipmentId'])))

    public stockpiles: IStockpileListItem[] = [];

    public selectedInventoryItem$ = new BehaviorSubject<IInventoryItemDetails | null>(null);

    public form = this.fb.group({
        header: this.fb.group({
            sourceReference: [''],
            issuedOn: [''],
            shippedOn: [''],
        }),
        currency: [''],
        partner: this.fb.group({
            partnerId: [''],
            legalName: [''],
            taxId: [''],
            address: [''],
        }),
        records: this.fb.array([])
    });

    public get records() {
        return this.form.get('records') as FormArray;
    }

    public get header() {
        return this.form.get('header') as FormGroup;
    }

    public get partner() {
        return this.form.get('partner') as FormGroup;
    }

    public total: number = 0;

    constructor(
        private route: ActivatedRoute,
        private shipmentService: ShipmentService,
        private inventoryItemService: InventoryItemService,
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.shipment$.subscribe(shipment => {
            this.header.patchValue({
                sourceReference: shipment.header.sourceReference,
                issuedOn: formatDate(shipment.header.issuedOn, 'yyyy-MM-dd', 'en'),
                shippedOn: formatDate(shipment.header.shippedOn, 'yyyy-MM-dd', 'en')
            });
            this.partner.patchValue({
                partnerId: shipment.partner.partnerId,
                legalName: shipment.partner.legalName,
                taxId: shipment.partner.taxId,
                address: shipment.partner.address
            });

            this.records.clear();
            shipment.records.forEach(record => {
                this.records.push(this.fb.group({
                    inventoryItemId: [record.inventoryItemId],
                    sku: [record.sku],
                    name: [record.name],
                    quantity: [record.quantity],
                    unitPrice: [record.unitPrice],
                    taxRate: [record.taxRate]
                }));
            });

            this.total = shipment.records.reduce((acc, record) => {
                return acc + record.quantity * record.unitPrice;
            }, 0);

            this.stockpiles = [{...shipment.stockpile, isDefault: true}];
        });
    }

    public handleAddRecord() {
        this.uiState = 'list-inventory-items';
    }

    public handleInventoryItemConfirmed($event: IInventoryItem) {
        this.inventoryItemService.getInventoryItem($event.inventoryItemId)
            .subscribe(inventoryItem => {
                this.selectedInventoryItem$.next(inventoryItem);
                this.uiState = 'add-record';
            });
    }

    public handleRecordAdded($event: AddedRecord) {
        this.addRecord($event);
        this.uiState = 'edit';
    }

    public handleSave() {
        if (!this.form.valid) {
            throw new Error('Form is invalid');
        }

        console.log(this.form.value);
    }

    public handleChangePartner() {
        this.uiState = 'list-partners';
    }

    public handlePartnerChanged($event: IPartner) {
        console.log($event);

        const taxId = `${$event.businessIdentifierType}: ${$event.businessIdentifierValue}`;
        const address = `${$event.street}, ${$event.postalCode}, ${$event.city}`;

        this.partner.patchValue({
            partnerId: $event.partnerId,
            legalName: $event.legalName,
            taxId,
            address
        });
        this.uiState = 'edit';
    }

    private addRecord(record: AddedRecord) {
        this.records.push(this.fb.group({
            inventoryItemId: [this.selectedInventoryItem$.value?.inventoryItemId],
            sku: [this.selectedInventoryItem$.value?.sku],
            name: [this.selectedInventoryItem$.value?.name],
            quantity: [record.quantity],
            unitPrice: [record.price],
            taxRate: [record.taxRate]
        }));
    }

}
