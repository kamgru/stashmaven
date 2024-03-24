import {Component} from '@angular/core';
import {
    AddStockpileRequest,
    IStockpile,
    StockpileService,
    UpdateStockpileRequest
} from "../../common/services/stockpile.service";
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
    public uiState: 'list' | 'edit' = 'list';
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
        if ($event.stockpileId === ''){
            const req = new AddStockpileRequest($event.name, $event.shortCode, $event.isDefault);
            this.stockpileService.addStockpile(req).subscribe(() => {
                this.stockpiles$ = this.stockpileService.listStockpiles();
                this.uiState = 'list';
            });
        }
        else {
            const req = new UpdateStockpileRequest($event.name, $event.shortCode, $event.isDefault);
            this.stockpileService.updateStockpile($event.stockpileId, req).subscribe(() => {
                this.stockpiles$ = this.stockpileService.listStockpiles();
                this.uiState = 'list';
            });
        }
    }
}
