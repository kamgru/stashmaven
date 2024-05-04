import {Component} from '@angular/core';
import {FaIconComponent} from "@fortawesome/angular-fontawesome";
import {ListStockpilesComponent} from "../../common/components/list-stockpiles/list-stockpiles.component";
import {ListInventoryComponent} from "../../common/components/list-inventory/list-inventory.component";
import {StockpileService} from "../../common/services/stockpile.service";
import {IInventoryItem} from "../../common/components/list-inventory/list-inventory.service";
import {AsyncPipe} from "@angular/common";

@Component({
    selector: 'app-inventory-items',
    standalone: true,
    imports: [
        FaIconComponent,
        ListStockpilesComponent,
        ListInventoryComponent,
        AsyncPipe
    ],
    templateUrl: './inventory-items.component.html'
})
export class InventoryItemsComponent {

    public stockpiles$ = this.stockpileService.listStockpiles();

    constructor(
        private stockpileService: StockpileService
    ) {
    }

    handleItemConfirmed($event: IInventoryItem) {

    }
}
