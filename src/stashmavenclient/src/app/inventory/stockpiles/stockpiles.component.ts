import {Component} from '@angular/core';
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {ListStockpilesComponent} from "../../common/components/list-stockpiles/list-stockpiles.component";
import {IStockpile} from "../../common/components/list-stockpiles/list-stockpiles.service";

@Component({
    selector: 'app-stockpiles',
    standalone: true,
    imports: [
        FaIconComponent,
        ListStockpilesComponent
    ],
    templateUrl: './stockpiles.component.html'
})
export class StockpilesComponent {

    public uiState: 'list' | 'add' = 'list';

    constructor(
        fa: FaIconLibrary,
    ) {
        fa.addIcons(faPlus);
    }

    handleItemConfirmed($event: IStockpile) {

    }
}
