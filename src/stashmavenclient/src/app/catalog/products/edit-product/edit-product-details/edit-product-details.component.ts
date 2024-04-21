import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {IGetProductDetailsResponse} from "../../../../common/services/product.service";

@Component({
    selector: 'app-edit-product-details',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './edit-product-details.component.html',
})
export class EditProductDetailsComponent implements OnInit {

    public productDetailsForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(5)]],
        unitOfMeasure: ['', Validators.required],
    });

    @Input({required: true})
    public editProductForm!: FormGroup;

    @Input({required: true})
    public unitsOfMeasure: string[] = [];

    @Input({required: true})
    public product!: IGetProductDetailsResponse;

    public get name(): FormControl<string> {
        return this.productDetailsForm.get('name') as FormControl<string>;
    }

    public get sku(): FormControl<string> {
        return this.productDetailsForm.get('sku') as FormControl<string>;
    }

    public get unitOfMeasure(): FormControl<string> {
        return this.productDetailsForm.get('unitOfMeasure') as FormControl<string>;
    }

    constructor(
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.name?.setValue(this.product.name);
        this.sku?.setValue(this.product.sku);
        this.unitOfMeasure?.setValue(this.product.unitOfMeasure);

        this.editProductForm.setControl('details', this.productDetailsForm);
    }
}
