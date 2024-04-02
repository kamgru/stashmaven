import {Component, OnInit} from '@angular/core';
import {ListProductsComponent} from "../../common/components/list-products/list-products.component";
import {AsyncPipe, NgIf} from "@angular/common";
import {ListInventoryComponent} from "../../common/components/list-inventory/list-inventory.component";
import {forkJoin, map} from "rxjs";
import {IStockpileListItem} from "../../common/IStockpileListItem";
import {IInventoryItem} from "../../common/components/list-inventory/list-inventory.service";
import {StockpileService} from "../../common/services/stockpile.service";

@Component({
    selector: 'app-stockpile-inventory',
    standalone: true,
    imports: [
        ListProductsComponent,
        NgIf,
        ListInventoryComponent,
        AsyncPipe
    ],
    templateUrl: './stockpile-inventory.component.html',
    styleUrl: './stockpile-inventory.component.css'
})
export class StockpileInventoryComponent implements OnInit {

    public stockpiles: IStockpileListItem[] = [];

    constructor(
        private stockpileService: StockpileService,
    ) {
    }

    ngOnInit(): void {
        forkJoin([
            this.stockpileService.listStockpiles(),
            this.stockpileService.getDefaultStockpileId()
        ])
            .pipe(
                map(([stockpiles, defaultStockpile]) => {
                        return stockpiles.items.map(x => {
                            return {
                                stockpileId: x.stockpileId,
                                name: x.name,
                                shortCode: x.shortCode,
                                isDefault: x.stockpileId === defaultStockpile.stockpileId
                            };
                        });
                    }
                ))
            .subscribe(x => {
                this.stockpiles = x;
                this.stockpiles.sort((a, b) => a.isDefault ? -1 : b.isDefault ? 1 : 0);
            })
    }

    handleItemSelected($event: IInventoryItem) {
       console.log($event);
    }
}
