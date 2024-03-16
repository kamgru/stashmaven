import {Component, EventEmitter, Input, Output} from "@angular/core";
import {NgForOf} from "@angular/common";
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {UnitOfMeasure} from "../../../../common/unitOfMeasure";
import {ICatalogItemDetails} from "../../../../common/services/catalog-item.service";

@Component({
    selector: 'app-catalog-item-base-props',
    standalone: true,
    templateUrl: './catalog-item-base-props.component.html',
    imports: [
        NgForOf,
        ReactiveFormsModule
    ]
})
export class CatalogItemBasePropsComponent {

    private _catalogItemDetails: ICatalogItemDetails = <ICatalogItemDetails>{};

    public unitsOfMeasure: string[] = Object.keys(UnitOfMeasure).filter(k => typeof UnitOfMeasure[k as any] === "number");

    public itemForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        sku: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        unitOfMeasure: ['', Validators.required],
    });

    @Input()
    public set CatalogItemDetails(value: ICatalogItemDetails) {
        this._catalogItemDetails = value;
        this.itemForm.setValue({
            name: value.name,
            sku: value.sku,
            unitOfMeasure: value.unitOfMeasure
        });
    }

    @Output()
    public OnFormSubmit = new EventEmitter<ICatalogItemDetails>();

    constructor(
        private formBuilder: FormBuilder) {
    }

    onSubmit() {
        if (!this.itemForm.valid) {
            return;
        }

        const evt = <ICatalogItemDetails>{
            catalogItemId: this._catalogItemDetails.catalogItemId,
            name: this.itemForm.value.name!,
            sku: this.itemForm.value.sku!,
            unitOfMeasure: this.itemForm.value.unitOfMeasure!.toString()
        };

        this.OnFormSubmit.emit(evt);

        this.itemForm.markAsPristine();
    }
}