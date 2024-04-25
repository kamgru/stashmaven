import {Component} from '@angular/core';
import * as ls from "./list-stockpiles.service";
import {ListItemsBaseComponent, ListItemsLayoutComponent} from "../list-items";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";

@Component({
    selector: 'app-list-stockpiles',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-stockpiles.component.html'
})
export class ListStockpilesComponent
    extends ListItemsBaseComponent<ls.IStockpile, ls.ListStockpilesRequest, ls.IListStockpilesResponse, ls.ListStockpilesService> {

    constructor(
        private listStockpiles: ls.ListStockpilesService,
    ) {
        super();
        this.bootstrap(listStockpiles);
    }
}
