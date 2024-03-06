import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddCatalogItemComponent} from "./add-catalog-item/add-catalog-item.component";
import {ListCatalogComponent} from "../../common/components/list-catalog/list-catalog.component";

@Component({
    selector: 'app-catalog-items',
    standalone: true,
    imports: [CommonModule, AddCatalogItemComponent, ListCatalogComponent, ListCatalogComponent],
    templateUrl: './catalog-items.component.html',
    styleUrls: ['./catalog-items.component.css']
})
export class CatalogItemsComponent {

    public uiState: 'list' | 'add' = 'list';


}
