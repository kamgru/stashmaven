import {Component, OnDestroy} from '@angular/core';
import * as li from "../components/list-items";
import * as br from "./list-brands.service";
import {AsyncPipe} from "@angular/common";
import {ListItemsLayoutComponent} from "../components/list-items";

@Component({
    selector: 'app-list-brands',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent
    ],
    templateUrl: './list-brands.component.html',
    styleUrl: './list-brands.component.css'
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
