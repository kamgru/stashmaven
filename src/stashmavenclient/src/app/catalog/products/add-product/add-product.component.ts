import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";

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
export class AddProductComponent implements OnInit {

    public addProductForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(5)]],
        unitOfMeasure: ['', Validators.required],
    });

    @Input({required: true})
    public unitsOfMeasure: string[] = [];

    @Output()
    public OnProductAdded = new EventEmitter<ProductAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    public get name(): FormControl<string>{
        return this.addProductForm.get('name') as FormControl<string>;
    }

    public get sku(): FormControl<string> {
        return this.addProductForm.get('sku') as FormControl<string>;
    }

    public get unitOfMeasure(): FormControl<string> {
        return this.addProductForm.get('unitOfMeasure') as FormControl<string>;
    }

    public ngOnInit() {
        this.addProductForm.get('unitOfMeasure')?.setValue(this.unitsOfMeasure[0]);
    }

    constructor(
        private formBuilder: FormBuilder,
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
