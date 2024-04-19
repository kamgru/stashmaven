import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddProductComponent, ProductAddedEvent} from "./add-product/add-product.component";
import {ListProductsComponent} from "../../common/components/list-products/list-products.component";
import {IProduct} from "../../common/components/list-products/list-products.service";
import {ActivatedRoute, Router} from "@angular/router";
import {FaIconLibrary, FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddProductRequest, ProductService} from "../../common/services/product.service";

@Component({
    selector: 'app-products',
    standalone: true,
    imports: [CommonModule, AddProductComponent, ListProductsComponent, ListProductsComponent, FontAwesomeModule],
    templateUrl: './products.component.html',
})
export class ProductsComponent {

    public uiState: 'list' | 'add' = 'list';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private productsService: ProductService,
        faLibrary: FaIconLibrary
    ) {
        faLibrary.addIcons(faPlus);
    }

    handleItemConfirmed($event: IProduct) {
        this.router.navigate([$event.productId], {relativeTo: this.route})
            .catch(err => console.error('Failed to navigate', err))
            .then(x => {
                console.log('Navigated', x);
            });
    }

    handleProductAdded($event: ProductAddedEvent) {
        this.productsService.add(new AddProductRequest(
            $event.name,
            $event.sku,
            $event.unitOfMeasure
        ))
            .subscribe(
                res => this.uiState = 'list'
            );
    }
}
