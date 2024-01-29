import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddCatalogItemComponent} from "./add-catalog-item/add-catalog-item.component";
import {ListCatalogItemsComponent} from "./list-catalog-items/list-catalog-items.component";
import {ListCatalogComponent} from "../../common/list-catalog/list-catalog.component";

enum UiState {
    Add = 'add',
    List = 'list',
    Edit = 'edit'
}

@Component({
    selector: 'app-catalog-items',
    standalone: true,
    imports: [CommonModule, AddCatalogItemComponent, ListCatalogItemsComponent, ListCatalogComponent],
    templateUrl: './catalog-items.component.html',
    styleUrls: ['./catalog-items.component.css']
})
export class CatalogItemsComponent {

    uiState = UiState.List;

    constructor() {
    }

    onAdd() {
        this.uiState = UiState.Add;
    }

    onCancel() {
        this.uiState = UiState.List;
    }

    onEdit() {
        this.uiState = UiState.Edit;
    }

    onSave() {
        this.uiState = UiState.List;
    }

    onDelete() {
        this.uiState = UiState.List;
    }

    protected readonly UiState = UiState;
}
