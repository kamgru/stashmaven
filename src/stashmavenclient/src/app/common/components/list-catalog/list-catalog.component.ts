import {Component, OnDestroy} from '@angular/core';
import {CommonModule} from '@angular/common';
import * as cat from "./list-catalog.service";
import *  as li from "../list-items";

@Component({
    selector: 'app-list-catalog',
    standalone: true,
    imports: [CommonModule, li.ListItemsBaseComponent, li.ListItemsLayoutComponent],
    templateUrl: './list-catalog.component.html',
    styleUrls: ['./list-catalog.component.css']
})
export class ListCatalogComponent
    extends li.ListItemsBaseComponent<
        cat.ICatalogItem,
        cat.ListCatalogItemsRequest,
        cat.IListCatalogItemsResponse,
        cat.ListCatalogService>
    implements OnDestroy {

    constructor(
        listCatalog: cat.ListCatalogService,
    ) {
        super();
        this.bootstrap(listCatalog);
    }
}
