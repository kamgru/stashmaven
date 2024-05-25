import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {AddProductComponent, ProductAddedEvent} from "./add-product/add-product.component";
import {ListProductsComponent} from "../../common/components/list-products/list-products.component";
import {IProduct} from "../../common/components/list-products/list-products.service";
import {ActivatedRoute, Router} from "@angular/router";
import {FaIconLibrary, FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddProductRequest, ProductService} from "../../common/services/product.service";
import {UnitOfMeasureService} from "../../common/services/unit-of-measure.service";
import {TaxDefinition, TaxDefinitionService} from "../../common/services/tax-definition.service";
import {combineLatest, map} from "rxjs";

@Component({
    selector: 'app-products',
    standalone: true,
    imports: [CommonModule, AddProductComponent, ListProductsComponent, ListProductsComponent, FontAwesomeModule],
    templateUrl: './products.component.html',
})
export class ProductsComponent {

    public uiState: 'list' | 'add' | 'edit' = 'list';

    private _unitsOfMeasure$ = this.unitOfMeasureService.getUnitsOfMeasure()
    private _taxes$ = this.taxesService.listAll()
        .pipe(map(x => x.items));

    public data$ = combineLatest([this._unitsOfMeasure$, this._taxes$])
        .pipe(map(([units, taxes]) => (
            <{ units: string[], taxes: TaxDefinition[] }>{units, taxes})));


    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private productsService: ProductService,
        private unitOfMeasureService: UnitOfMeasureService,
        private taxesService: TaxDefinitionService,
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
            $event.unitOfMeasure,
            $event.taxDefinitionId
        ))
            .subscribe(
                res => {
                    this.router.navigate([res.productId], {relativeTo: this.route})
                        .catch(err => console.error('Failed to navigate', err))
                        .then(x => {
                            console.log('Navigated', x);
                        });
                }
            );
    }
}
