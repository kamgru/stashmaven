import {Component, Input} from '@angular/core';
import {IGetProductStockpilesResponse} from "../../../../../common/services/product.service";
import {JsonPipe} from "@angular/common";

@Component({
    selector: 'app-stockpiles',
    standalone: true,
    imports: [
        JsonPipe
    ],
    templateUrl: './stockpiles.component.html'
})
export class StockpilesComponent {

    @Input({required: true})
    public stockpiles!: IGetProductStockpilesResponse;
}
