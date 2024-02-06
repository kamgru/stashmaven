import {Component, OnInit} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {ListInventoryComponent} from "../../common/list-inventory/list-inventory.component";
import {ListShipmentsComponent} from "../../common/list-shipments/list-shipments.component";
import {forkJoin, map} from "rxjs";
import {StockpileService} from "../../common/services/stockpile.service";
import {IStockpileListItem} from "../../common/IStockpileListItem";

@Component({
    selector: 'app-stockpile-shipments',
    standalone: true,
    imports: [
        AsyncPipe,
        ListInventoryComponent,
        ListShipmentsComponent
    ],
    templateUrl: './stockpile-shipments.component.html',
    styleUrl: './stockpile-shipments.component.css'
})
export class StockpileShipmentsComponent implements OnInit {

    public stockpiles: IStockpileListItem[] = [];

    constructor(
        private stockpileService: StockpileService) {
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
}
