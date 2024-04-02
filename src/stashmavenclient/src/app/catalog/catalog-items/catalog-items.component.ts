import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddCatalogItemComponent, ProductAddedEvent} from "./add-catalog-item/add-catalog-item.component";
import {ListCatalogComponent} from "../../common/components/list-catalog/list-catalog.component";
import {ICatalogItem} from "../../common/components/list-catalog/list-catalog.service";
import {ActivatedRoute, Router} from "@angular/router";
import {FaIconLibrary, FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddCatalogItemRequest, CatalogItemService} from "../../common/services/catalog-item.service";

@Component({
    selector: 'app-catalog-items',
    standalone: true,
    imports: [CommonModule, AddCatalogItemComponent, ListCatalogComponent, ListCatalogComponent, FontAwesomeModule],
    templateUrl: './catalog-items.component.html',
})
export class CatalogItemsComponent {

    public uiState: 'list' | 'add' = 'add';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private catalogItemService: CatalogItemService,
        faLibrary: FaIconLibrary
    ) {
        faLibrary.addIcons(faPlus);
    }

    handleItemConfirmed($event: ICatalogItem) {
        this.router.navigate([$event.catalogItemId], {relativeTo: this.route})
            .catch(err => console.error('Failed to navigate', err))
            .then(x => {
                console.log('Navigated', x);
            });
    }

    handleProductAdded($event: ProductAddedEvent) {
        this.catalogItemService.add(new AddCatalogItemRequest(
            $event.name,
            $event.sku,
            $event.unitOfMeasure
        ))
            .subscribe(
                res => this.uiState = 'list'
            );
    }
}
