import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {tap} from "rxjs";
import * as inv from "./list-inventory.service";
import {AsyncPipe} from "@angular/common";
import {IStockpileListItem} from "../../IStockpileListItem";
import {FormsModule} from "@angular/forms";
import * as li from "../list-items";
import {DropdownComponent} from "../dropdown/dropdown.component";
import {ISelectOption, SelectComponent} from "../select/select.component";

@Component({
    selector: 'app-list-inventory',
    standalone: true,
    imports: [
        AsyncPipe,
        FormsModule,
        li.ListItemsLayoutComponent,
        DropdownComponent,
        SelectComponent
    ],
    templateUrl: './list-inventory.component.html',
    styleUrl: './list-inventory.component.css',
})
export class ListInventoryComponent
    extends li.ListItemsBaseComponent<inv.IInventoryItem, inv.ListInventoryItemsRequest, inv.IListInventoryItemsResponse, inv.ListInventoryService>
    implements OnInit, OnDestroy {

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    public selectOptions: ISelectOption[] = [];
    public selectedOption: ISelectOption | null = null;

    constructor(
        private listInventory: inv.ListInventoryService,
    ) {
        super();
        this.bootstrap(listInventory);

        this.listResponse$ = this.listInventory.items$
            .pipe(
                tap(x => {
                    const selectedStockpile = this.stockpiles.find(y => y.stockpileId === x.stockpileId);
                    this.selectedOption = {value: x.stockpileId, label: selectedStockpile?.name ?? ''};
                })
            );

    }

    ngOnInit(){
        this.selectOptions = this.stockpiles.map(x => ({value: x.stockpileId, label: x.name}));
    }

    handleStockpileChanged(value: ISelectOption) {
        const stockpile = this.stockpiles.find(x => x.stockpileId === value.value);
        this.listInventory.changeStockpile(stockpile?.stockpileId ?? '');
    }
}
