import {Component, OnInit} from '@angular/core';
import {ListCatalogComponent} from "../../common/list-catalog/list-catalog.component";
import {AsyncPipe, NgIf} from "@angular/common";
import {ListInventoryComponent} from "../../common/list-inventory/list-inventory.component";
import {StockpileInventoryService} from "./stockpile-inventory.service";
import {forkJoin, map, mergeMap, Observable, of, switchMap} from "rxjs";
import {merge} from "rxjs/internal/operators/merge";

interface IStockpileListItem {
    stockpileId: string;
    name: string;
    shortCode: string;
    isDefault: boolean;
}

@Component({
    selector: 'app-stockpile-inventory',
    standalone: true,
    imports: [
        ListCatalogComponent,
        NgIf,
        ListInventoryComponent,
        AsyncPipe
    ],
    templateUrl: './stockpile-inventory.component.html',
    styleUrl: './stockpile-inventory.component.css'
})
export class StockpileInventoryComponent implements OnInit {

    public stockpiles$: Observable<IStockpileListItem[]> = of([]);

    constructor(
        private stockpileInventoryService: StockpileInventoryService,
    ) {
    }

    ngOnInit(): void {
        this.stockpiles$ = forkJoin([
            this.stockpileInventoryService.listStockpiles(),
            this.stockpileInventoryService.getDefaultStockpileId()
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
                ));
    }
}
