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
    selector: 'app-add-product',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './add-product.component.html',
})
export class AddProductComponent {

    public unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    public addProductForm = this.formBuilder.group({
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
        if (!this.addProductForm.valid) {
            return;
        }

        const evt = new ProductAddedEvent(
            this.addProductForm.value.name!,
            this.addProductForm.value.sku!,
            this.addProductForm.value.unitOfMeasure!.toString()
        );

        this.OnProductAdded.emit(evt);
    }
}
