import {Component} from '@angular/core';
import {IStockpile, StockpileService} from "../../common/services/stockpile.service";
import {AsyncPipe} from "@angular/common";
import {EditStockpileComponent} from "./edit-stockpile/edit-stockpile.component";

@Component({
    selector: 'app-stockpiles',
    standalone: true,
    imports: [
        AsyncPipe,
        EditStockpileComponent
    ],
    templateUrl: './stockpiles.component.html',
    styleUrl: './stockpiles.component.css'
})
export class StockpilesComponent {

    public stockpiles$ = this.stockpileService.listStockpiles();
    public uiState: 'list' | 'edit' | 'add' = 'list';
    public selectedStockpile: IStockpile | null = null;

    constructor(
        private stockpileService: StockpileService,
    ) {
    }

    handleStockpileClicked(stockpile: IStockpile) {
        this.selectedStockpile = stockpile;
        this.uiState = 'edit';
    }

    handleEditCompleted($event: IStockpile) {
        

    }
}
