import {Component, OnDestroy} from '@angular/core';
import {CommonModule} from '@angular/common';
import * as cat from "./list-products.service";
import *  as li from "../list-items";

@Component({
    selector: 'app-list-products',
    standalone: true,
    imports: [CommonModule, li.ListItemsBaseComponent, li.ListItemsLayoutComponent],
    templateUrl: './list-products.component.html',
    styleUrls: ['./list-products.component.css']
})
export class ListProductsComponent
    extends li.ListItemsBaseComponent<
        cat.IProduct,
        cat.ListProductsRequest,
        cat.IListProductsResponse,
        cat.ListProductsService>
    implements OnDestroy {

    constructor(
        listProducts: cat.ListProductsService,
    ) {
        super();
        this.bootstrap(listProducts);
    }
}
