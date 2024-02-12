import {Component, OnDestroy} from '@angular/core';
import {CommonModule} from '@angular/common';
import {
    ICatalogItem,
    IListCatalogItemsResponse,
    ListCatalogItemsRequest,
    ListCatalogService
} from "./list-catalog.service";
import {ListItemsComponentBase} from "../components/list-items-component-base.component";
import {ListPagingInfoComponent} from "../components/list-paging-info/list-paging-info.component";

@Component({
    selector: 'app-list-catalog',
    standalone: true,
    imports: [CommonModule, ListPagingInfoComponent],
    templateUrl: './list-catalog.component.html',
    styleUrls: ['./list-catalog.component.css']
})
export class ListCatalogComponent
    extends ListItemsComponentBase<ICatalogItem, ListCatalogItemsRequest, IListCatalogItemsResponse, ListCatalogService>
    implements OnDestroy {

    constructor(
        private listCatalog: ListCatalogService,
    ) {
        super();
        this.bootstrap(listCatalog);
        this.items$ = this.listCatalog.items$;
    }
}
