import {Component, Input, OnDestroy} from '@angular/core';
import {tap} from "rxjs";
import {
    IInventoryItem,
    IListInventoryItemsResponse,
    ListInventoryItemsRequest,
    ListInventoryService
} from "./list-inventory.service";
import {AsyncPipe} from "@angular/common";
import {IStockpileListItem} from "../IStockpileListItem";
import {FormsModule} from "@angular/forms";
import {ListItemsBaseComponent} from "../components/list-items/list-items-base/list-items-base.component";
import {ListSearchInputComponent} from "../components/list-items/list-search-input/list-search-input.component";

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe,
        FormsModule,
        ListSearchInputComponent
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css',
})
export class ListInventoryComponent
    extends ListItemsBaseComponent<IInventoryItem, ListInventoryItemsRequest, IListInventoryItemsResponse, ListInventoryService>
    implements OnDestroy {

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    public selectedStockpile?: IStockpileListItem;

    constructor(
        private listInventory: ListInventoryService,
    ) {
        super();
        this.bootstrap(listInventory);

        this.listResponse$ = this.listInventory.items$
            .pipe(
                tap(x => this.selectedStockpile = this.stockpiles.find(y => y.stockpileId === x.stockpileId))
            );
    }

    handleStockpileChanged(value: string) {
        this.listInventory.changeStockpile(value);
    }
}
