import {AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {IInventoryItemDetails} from "../../../../common/services/inventory-item.service";
import {FormBuilder, ReactiveFormsModule} from "@angular/forms";

export class AddedRecord {
    constructor(
        public inventoryItemId: string,
        public sku: string,
        public name: string,
        public quantity: number,
        public price: number,
        public taxRate: number) {
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
export class AddRecordComponent implements OnInit, AfterViewInit {

    @Input({required: true})
    public inventoryItem?: IInventoryItemDetails;

    @Output()
    public OnRecordAdded = new EventEmitter<AddedRecord>();

    @Output()
    public OnCancel = new EventEmitter<void>();

    @ViewChild('quantityInput', {static: true})
    public quantityInput!: ElementRef;

    public form = this.fb.group({
        quantity: [''],
        price: [0],
        taxRate: [0],
    });

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        if (this.inventoryItem) {
            this.form.patchValue({
                price: this.inventoryItem.lastPurchasePrice,
                taxRate: this.inventoryItem.buyTaxRate
            });
        }
    }

    public ngAfterViewInit() {
        this.quantityInput.nativeElement.focus();
    }

    public handleSubmit() {
        if (!this.form.valid) {
            throw new Error('Form is invalid');
        }

        const addedRecord = new AddedRecord(
            this.inventoryItem!.inventoryItemId,
            this.inventoryItem!.sku,
            this.inventoryItem!.name,
            +this.form.value.quantity!,
            this.form.value.price!,
            this.form.value.taxRate!
        );
        this.OnRecordAdded.emit(addedRecord);
    }

    public handleCancel() {
        this.OnCancel.emit();
    }
}
