import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";
import {TaxDefinition} from "../../../common/services/tax-definition.service";

export class ProductAddedEvent {
    constructor(
        public readonly name: string,
        public readonly sku: string,
        public readonly unitOfMeasure: string,
        public readonly taxDefinitionId: string
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
        tax: ['', Validators.required]
    });

    @Input({required: true})
    public unitsOfMeasure: string[] = [];

    @Input({required: true})
    public taxDefinitions: TaxDefinition[] = [];

    @Output()
    public OnProductAdded = new EventEmitter<ProductAddedEvent>();

    @Output()
    public OnCancelled = new EventEmitter<void>();

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    public get name(): FormControl<string>{
        return this.addProductForm.get('name') as FormControl<string>;
    }

    public get sku(): FormControl<string> {
        return this.addProductForm.get('sku') as FormControl<string>;
    }

    public get unitOfMeasure(): FormControl<string> {
        return this.addProductForm.get('unitOfMeasure') as FormControl<string>;
    }

    public get taxDefinition(): FormControl<string> {
        return this.addProductForm.get('tax') as FormControl<string>
    }

    public ngOnInit() {
        this.addProductForm.get('unitOfMeasure')?.setValue(this.unitsOfMeasure[0]);
        this.taxDefinition.setValue(this.taxDefinitions[0].name);
        this.nameInput!.nativeElement.focus();
    }

    constructor(
        private formBuilder: FormBuilder,
    ) {
    }

    public handleSubmit() {
        if (!this.addProductForm.valid) {
            return;
        }

        const tax = this.taxDefinitions.find(
            x => x.taxDefinitionId === this.taxDefinition.value);

        if (!tax) {
            throw new Error('Tax definition not found');
        }

        const evt = new ProductAddedEvent(
            this.addProductForm.value.name!,
            this.addProductForm.value.sku!,
            this.addProductForm.value.unitOfMeasure!.toString(),
            tax.taxDefinitionId
        );

        this.OnProductAdded.emit(evt);
    }
}
