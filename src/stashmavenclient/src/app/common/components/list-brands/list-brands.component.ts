import {Component, OnDestroy} from '@angular/core';
import * as li from "../list-items";
import * as br from "./list-brands.service";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";
import {ListItemsLayoutComponent} from "../list-items";

@Component({
    selector: 'app-list-brands',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-brands.component.html',
})
export class ListBrandsComponent
    extends li.ListItemsBaseComponent<br.IBrandItem, br.ListBrandsRequest, br.IListBrandsResponse, br.ListBrandsService>
    implements OnDestroy {

    constructor(
        listBrands: br.ListBrandsService,
    ) {
        super();
        this.bootstrap(listBrands);
    }
}
