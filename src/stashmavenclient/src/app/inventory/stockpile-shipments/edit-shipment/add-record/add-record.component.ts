import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";
import {IInventoryItemDetails} from "../../../../common/services/inventory-item.service";

export class RecordAdded{
    constructor(
        public inventoryItem: IInventoryItemDetails,
        public quantity: number,
        public unitPrice: number
    ) {
    }
}

@Component({
    selector: 'app-add-record',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './add-record.component.html',
    styleUrl: './add-record.component.css'
})
export class AddRecordComponent implements OnInit {

    @Input()
    public shipmentKind: 'inbound' | 'outbound' | null = null;

    @Input()
    public inventoryItem: IInventoryItemDetails | null = null;

    @Output()
    public OnRecordAdded = new EventEmitter<RecordAdded>();


    public itemForm = this.formBuilder.group({
        sku: new FormControl({value: this.inventoryItem?.sku, disabled: true}),
        name: new FormControl({value: this.inventoryItem?.name, disabled: true}),
        quantity: [1, [Validators.required, Validators.min(1)]],
        unitPrice: [0, [Validators.required, Validators.min(0.01)]]
    });

    constructor(
        private formBuilder: FormBuilder,
    ) {
    }

    ngOnInit() {
        if (this.inventoryItem) {
            const unitPrice = this.shipmentKind === 'inbound'
                ? this.inventoryItem.lastPurchasePrice
                : this.inventoryItem.sellPrice;
            this.itemForm.patchValue({
                unitPrice: unitPrice,
                sku: this.inventoryItem.sku,
                name: this.inventoryItem.name
            });
        }
    }

    handleSubmit() {
        if (this.itemForm.invalid) {
            return;
        }

        if (!this.inventoryItem) {
            throw new Error('inventoryItem is not set');
        }

        this.OnRecordAdded.emit(new RecordAdded(
            this.inventoryItem,
            this.itemForm.value.quantity!,
            this.itemForm.value.unitPrice!
        ));
    }
}
