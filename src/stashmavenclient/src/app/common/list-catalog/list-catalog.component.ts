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
import {ListSearchInputComponent} from "../components/list-search-input/list-search-input.component";
import {ListPageSizeSelectComponent} from "../components/list-page-size-select/list-page-size-select.component";

@Component({
    selector: 'app-list-catalog',
    standalone: true,
    imports: [CommonModule, ListPagingInfoComponent, ListSearchInputComponent, ListPageSizeSelectComponent],
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

    override search = (value: string) => {
        this.listCatalog.search(value);
    };
}
