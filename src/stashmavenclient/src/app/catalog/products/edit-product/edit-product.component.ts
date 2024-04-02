import {Component, OnInit} from '@angular/core';
import {ProductBasePropsComponent} from "./product-base-props/product-base-props.component";
import {ActivatedRoute} from "@angular/router";
import {
    ProductService, ChangeProductStockpileAvailabilityRequest,
    IProductDetails,
    IGetProductStockpilesResponse, StockpileAvailability
} from "../../../common/services/product.service";
import {Observable} from "rxjs";
import {AsyncPipe} from "@angular/common";
import {
    ProductAvailability,
    ProductStockpilesComponent
} from "./product-stockpiles/product-stockpiles.component";

@Component({
    selector: 'app-edit-product',
    standalone: true,
    imports: [
        ProductBasePropsComponent,
        AsyncPipe,
        ProductStockpilesComponent
    ],
    templateUrl: './edit-product.component.html',
})
export class EditProductComponent implements OnInit {

    private _id?: string;

    public product$?: Observable<IProductDetails>;
    public stockpiles$?: Observable<IGetProductStockpilesResponse>;

    constructor(
        private route: ActivatedRoute,
        private productService: ProductService
    ) {
    }

    ngOnInit() {
        const id = this.route.snapshot.paramMap.get('id');

        if (!id) {
            throw new Error('Catalog item id not found');
        }

        this._id = id;

        this.product$ = this.productService.getProductDetails(this._id);
        this.stockpiles$ = this.productService.getProductStockpiles(this._id);
    }

    handleSubmitDetails($event: IProductDetails) {
        this.productService.updateProductDetails($event)
            .subscribe(() => {
            });
    }

    handleSubmitAvailability($event: ProductAvailability[]) {
        const availability: StockpileAvailability[] = [];

        for (let item of $event) {
            availability.push(new StockpileAvailability(item.stockpileId, item.available));
        }

        const req = new ChangeProductStockpileAvailabilityRequest(this._id!, availability);

        this.productService.changeProductStockpileAvailability(req)
            .subscribe();
    }
}
