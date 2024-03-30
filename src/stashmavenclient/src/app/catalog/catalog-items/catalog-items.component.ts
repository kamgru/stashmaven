import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddCatalogItemComponent} from "./add-catalog-item/add-catalog-item.component";
import {ListCatalogComponent} from "../../common/components/list-catalog/list-catalog.component";
import {ICatalogItem} from "../../common/components/list-catalog/list-catalog.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
    selector: 'app-catalog-items',
    standalone: true,
    imports: [CommonModule, AddCatalogItemComponent, ListCatalogComponent, ListCatalogComponent],
    templateUrl: './catalog-items.component.html',
})
export class CatalogItemsComponent {

    public uiState: 'list' | 'add' = 'list';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
    ) {
    }

    handleItemConfirmed($event: ICatalogItem) {
        this.router.navigate([$event.catalogItemId], {relativeTo: this.route})
            .catch(err => console.error('Failed to navigate', err))
            .then(x => {
                console.log('Navigated', x);
            });
    }
}
