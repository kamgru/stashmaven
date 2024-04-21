import {Component, EventEmitter, Input, Output} from '@angular/core';
import {
    IGetProductDetailsResponse,
    IGetProductStockpilesResponse,
    ProductService
} from "../../../common/services/product.service";
import {ActivatedRoute} from "@angular/router";
import {combineLatest, map, switchMap} from "rxjs";
import {EditProductDetailsComponent} from "./edit-product-details/edit-product-details.component";
import {AsyncPipe} from "@angular/common";
import {FormBuilder, ReactiveFormsModule} from "@angular/forms";
import {UnitOfMeasureService} from "../../../common/services/unit-of-measure.service";
import {EditProductAdvancedComponent} from "./edit-product-advanced/edit-product-advanced.component";
import {EditProductOrganizeComponent} from "./edit-product-organize/edit-product-organize.component";

export class ProductEditedEvent {
    constructor(
        public readonly id: string,
        public readonly name: string,
        public readonly sku: string,
        public readonly unitOfMeasure: string
    ) {
    }
}

export class EditProductData {
    public details!: IGetProductDetailsResponse
    public stockpiles!: IGetProductStockpilesResponse
    public unitsOfMeasure!: string[]
}

@Component({
    selector: 'app-edit-product',
    standalone: true,
    imports: [
        EditProductDetailsComponent,
        AsyncPipe,
        ReactiveFormsModule,
        EditProductAdvancedComponent,
        EditProductOrganizeComponent
    ],
    templateUrl: './edit-product.component.html',
})
export class EditProductComponent {

    public editProductForm = this.fb.group({
        details: this.fb.group({})
    });

    public product$ = this.route.params.pipe(
        switchMap(params => this.productService.getProductDetails(params['id']))
    );

    public stockpiles$ = this.route.params.pipe(
        switchMap(params => this.productService.getProductStockpiles(params['id']))
    );

    public unitsOfMeasure$ = this.unitOfMeasureService.getUnitsOfMeasure();

    public data$ = combineLatest([this.product$, this.stockpiles$, this.unitsOfMeasure$])
        .pipe(map(([details, stockpiles, unitsOfMeasure]) => (
            <EditProductData>{details, stockpiles, unitsOfMeasure})));

    @Output()
    public OnProductEdited = new EventEmitter<ProductEditedEvent>();
    OnCancelled: any;

    constructor(
        private route: ActivatedRoute,
        private productService: ProductService,
        private unitOfMeasureService: UnitOfMeasureService,
        private fb: FormBuilder
    ) {
    }

    handleSubmit() {
        console.log(this.editProductForm)
    }
}
