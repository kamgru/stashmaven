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
import {ListItemsComponentBase} from "../components/list-items-component-base.component";

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe,
        FormsModule
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css',
})
export class ListInventoryComponent
    extends ListItemsComponentBase<IInventoryItem, ListInventoryItemsRequest, IListInventoryItemsResponse, ListInventoryService>
    implements OnDestroy {

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    public selectedStockpile?: IStockpileListItem;

    constructor(
        private listInventory: ListInventoryService,
    ) {
        super();
        this.bootstrap(listInventory);

        this.items$ = this.listInventory.items$
            .pipe(
                tap(x => this.selectedStockpile = this.stockpiles.find(y => y.stockpileId === x.stockpileId))
            );
    }

    handleStockpileChanged(value: string) {
        this.listInventory.changeStockpile(value);
    }
}
