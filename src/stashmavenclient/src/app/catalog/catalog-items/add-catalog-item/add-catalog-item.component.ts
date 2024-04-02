import {Component, EventEmitter, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {UnitOfMeasure} from "../../../common/unitOfMeasure";

export class ProductAddedEvent {
    constructor(
        public readonly name: string,
        public readonly sku: string,
        public readonly unitOfMeasure: string
    ) {
    }
}

@Component({
    selector: 'app-add-catalog-item',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './add-catalog-item.component.html',
})
export class AddCatalogItemComponent {

    public unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    public addCatalogItemForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        unitOfMeasure: [this.unitsOfMeasure, Validators.required],
    });

    @Output()
    public OnProductAdded = new EventEmitter<ProductAddedEvent>();
    @Output()
    public OnCancelled = new EventEmitter<void>();

    constructor(
        private formBuilder: FormBuilder
    ) {
    }

    public handleSubmit() {
        if (!this.addCatalogItemForm.valid) {
            return;
        }

        const evt = new ProductAddedEvent(
            this.addCatalogItemForm.value.name!,
            this.addCatalogItemForm.value.sku!,
            this.addCatalogItemForm.value.unitOfMeasure!.toString()
        );

        this.OnProductAdded.emit(evt);
    }
}
