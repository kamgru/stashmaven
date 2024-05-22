import {Component, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {formatDate} from "@angular/common";
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {faDeleteLeft, faEdit, faPlus, faTrash} from '@fortawesome/free-solid-svg-icons';
import {IPartner, ListPartnersComponent} from "../../../common/components/list-partners";
import {ListInventoryComponent} from "../../../common/components/list-inventory/list-inventory.component";


export class Shipment {
    constructor(
        public shipmentId: string | null,
        public sequenceNumber: string | null,
        public acceptance: string,
        public currency: string,
        public sourceReference: string,
        public issuedOn: string,
        public shippedOn: string,
        public kind: {
            shipmentKindId: string,
            name: string,
            shortCode: string,
            direction: string
        },
        public stockpile: {
            stockpileId: string,
            name: string,
            shortCode: string,
            isDefault: boolean
        },
        public partner: {
            partnerId: string,
            customIdentifier: string,
            legalName: string,
            address: string,
            businessId: {
                value: string,
                type: string
            }
        },
        public records: {
            inventoryItemId: string,
            quantity: number,
            unitPrice: number,
            sku: string,
            name: string,
            taxRate: number,
            unitOfMeasure: string
        }[]
    ) {
    }
}

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        FaIconComponent,
        ListPartnersComponent,
        ListInventoryComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styles: [
        `.delete-icon {
          cursor: pointer;
        }

        .delete-icon:hover {
          color: red;
        }
        `]
})
export class EditShipmentComponent implements OnInit {

    public uiState: 'edit' | 'list-inventory-items' | 'add-record' | 'list-partners' = 'edit';

    public shipment = new Shipment(
        'dbb1baf9-7238-4712-8418-9c72278711d4',
        'PZ/538/M1/2024',
        'draft',
        'PLN',
        'FV-06/08324001/24',
        '2024-05-22',
        '2024-05-21',
        {
            shipmentKindId: '0fffc2fc-aa95-4550-9798-466cd9ee8555',
            name: 'Przyjęcie z Zewnątrz',
            shortCode: 'PZ',
            direction: 'In'
        },
        {
            stockpileId: '15bbae2c-395f-4bd2-91f9-594fdf920ae4',
            name: 'Magazyn Główny',
            shortCode: 'M1',
            isDefault: true
        },
        {
            partnerId: '40076fdd-48c4-442a-b51b-9b728cb094b0',
            customIdentifier: 'Hawed',
            legalName: 'Hawed Sp. z o.o.',
            businessId: {
                value: '6572268066',
                type: 'nip'
            },
            address: 'Górna 25, 25-415, Kielce, Polska',
        },
        [{
            inventoryItemId: 'f6b1c4d1-5b2d-4b3b-8c2b-8b1c4d1f6b1c',
            quantity: 10,
            unitPrice: 21.37,
            sku: 'JB122',
            name: 'Kiełbasa Toruńska',
            taxRate: 23,
            unitOfMeasure: 'kg'
        }, {
            inventoryItemId: 't6b1c4d1-5b2d-4b3b-8c2b-8b1c4d1f6b1c',
            quantity: 31,
            unitPrice: 11.99,
            sku: 'S310',
            name: 'Kiszka wiejska',
            taxRate: 23,
            unitOfMeasure: 'kg'
        }]
    );

    public editShipmentForm = this.fb.group({
        header: this.fb.group({
            sourceReference: [''],
            issuedOn: [''],
            shippedOn: ['']
        }),
        partner: this.fb.group({
            partnerId: [''],
        }),
        records: this.fb.array([])
    })

    public get header(): FormGroup {
        return this.editShipmentForm.get('header') as FormGroup;
    }

    public get records(): FormArray {
        return this.editShipmentForm.get('records') as FormArray;
    }

    constructor(
        private fb: FormBuilder,
        fa: FaIconLibrary
    ) {
        fa.addIcons(faDeleteLeft, faPlus);
    }

    public ngOnInit() {
        this.header.get('sourceReference')?.setValue(this.shipment.sourceReference);
        this.header.get('issuedOn')?.setValue(
            formatDate(this.shipment.issuedOn, 'yyyy-MM-dd', 'en')
        );
        this.header.get('shippedOn')?.setValue(
            formatDate(this.shipment.shippedOn, 'yyyy-MM-dd', 'en')
        );

        this.shipment.records.forEach(record => {
            this.records.push(this.fb.group({
                inventoryItemId: [record.inventoryItemId],
                quantity: [record.quantity],
                unitPrice: [record.unitPrice],
                sku: [record.sku],
                name: [record.name],
                taxRate: [record.taxRate],
                unitOfMeasure: [record.unitOfMeasure]
            }));
        });
    }

    handleDeleteRecordClicked(index: number) {
        this.records.removeAt(index);
    }

    handleChangePartnerClicked() {
        this.uiState = 'list-partners';
    }

    handlePartnerSelected($event: IPartner) {
       this.shipment.partner = {
              partnerId: $event.partnerId,
              customIdentifier: $event.customIdentifier,
              legalName: $event.legalName,
              businessId: {
                type: $event.businessIdentifierType,
                value: $event.businessIdentifierValue
              },
              address: `${$event.street}, ${$event.postalCode}, ${$event.city}`
       };

       this.editShipmentForm.get('partner')?.get('partnerId')?.setValue($event.partnerId);

       this.uiState = 'edit';
    }

    handleAddRecordClicked() {
        this.uiState = 'list-inventory-items';
    }


}
