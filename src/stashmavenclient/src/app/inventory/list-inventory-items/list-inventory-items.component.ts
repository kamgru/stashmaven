import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {SuperGridColumn, SuperGridComponent} from "../../common/super-grid/super-grid.component";
import {ListInventoryItemsService} from "./list-inventory-items.service";

@Component({
    selector: 'app-list-inventory-items',
    standalone: true,
    imports: [CommonModule, SuperGridComponent],
    templateUrl: './list-inventory-items.component.html',
    styleUrls: ['./list-inventory-items.component.css']
})
export class ListInventoryItemsComponent {

    columns: SuperGridColumn[] = [
        new SuperGridColumn('id', 'ID', false, false ),
        new SuperGridColumn('name', 'Name', true, true),
        new SuperGridColumn('sku', 'SKU', true, true),
        new SuperGridColumn('purchasePrice', 'Purchase Price', false, true),
        new SuperGridColumn('quantity', 'Quantity', false, true),
    ];

    constructor(
        public listService: ListInventoryItemsService
    ) {
    }
}
