import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import * as cat from "./list-products.service";
import *  as li from "../list-items";

@Component({
    selector: 'app-list-products',
    standalone: true,
    imports: [CommonModule, li.ListItemsBaseComponent, li.ListItemsLayoutComponent],
    templateUrl: './list-products.component.html',
})
export class ListProductsComponent
    extends li.ListItemsBaseComponent<
        cat.IProduct,
        cat.ListProductsRequest,
        cat.IListProductsResponse,
        cat.ListProductsService> {

    constructor(
        listProducts: cat.ListProductsService,
    ) {
        super();
        this.bootstrap(listProducts);
    }
}
