import {Component, EventEmitter, Input, Output} from "@angular/core";
import {NgForOf} from "@angular/common";
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {UnitOfMeasure} from "../../../../common/unitOfMeasure";
import {IProductDetails} from "../../../../common/services/product.service";

@Component({
    selector: 'app-product-base-props',
    standalone: true,
    templateUrl: './product-base-props.component.html',
    imports: [
        NgForOf,
        ReactiveFormsModule
    ]
})
export class ProductBasePropsComponent {

    private _productDetails: IProductDetails = <IProductDetails>{};

    public unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    public itemForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        unitOfMeasure: ['', Validators.required],
    });

    @Input()
    public set ProductDetails(value: IProductDetails) {
        this._productDetails = value;
        this.itemForm.setValue({
            name: value.name,
            sku: value.sku,
            unitOfMeasure: value.unitOfMeasure
        });
    }

    @Output()
    public OnFormSubmit = new EventEmitter<IProductDetails>();

    constructor(
        private formBuilder: FormBuilder) {
    }

    onSubmit() {
        if (!this.itemForm.valid) {
            return;
        }

        const evt = <IProductDetails>{
            productId: this._productDetails.productId,
            name: this.itemForm.value.name!,
            sku: this.itemForm.value.sku!,
            unitOfMeasure: this.itemForm.value.unitOfMeasure!.toString()
        };

        this.OnFormSubmit.emit(evt);

        this.itemForm.markAsPristine();
    }
}