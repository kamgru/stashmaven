import {Component, Input} from '@angular/core';
import {StockpilesComponent} from "./stockpiles/stockpiles.component";
import {IGetProductStockpilesResponse} from "../../../../common/services/product.service";

@Component({
    selector: 'app-edit-product-advanced',
    standalone: true,
    imports: [
        StockpilesComponent
    ],
    templateUrl: './edit-product-advanced.component.html',
})
export class EditProductAdvancedComponent {

    @Input({required: true})
    public stockpiles!: IGetProductStockpilesResponse;

}
