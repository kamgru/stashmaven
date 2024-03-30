import {Component, OnInit} from '@angular/core';
import {CatalogItemBasePropsComponent} from "./catalog-item-base-props/catalog-item-base-props.component";
import {ActivatedRoute} from "@angular/router";
import {
    CatalogItemService, ChangeCatalogItemStockpileAvailabilityRequest,
    ICatalogItemDetails,
    IGetCatalogItemStockpilesResponse, StockpileAvailability
} from "../../../common/services/catalog-item.service";
import {Observable} from "rxjs";
import {AsyncPipe} from "@angular/common";
import {
    CatalogItemAvailability,
    CatalogItemStockpilesComponent
} from "./catalog-item-stockpiles/catalog-item-stockpiles.component";

@Component({
    selector: 'app-edit-catalog-item',
    standalone: true,
    imports: [
        CatalogItemBasePropsComponent,
        AsyncPipe,
        CatalogItemStockpilesComponent
    ],
    templateUrl: './edit-catalog-item.component.html',
})
export class EditCatalogItemComponent implements OnInit {

    private _id?: string;

    public catalogItem$?: Observable<ICatalogItemDetails>;
    public stockpiles$?: Observable<IGetCatalogItemStockpilesResponse>;

    constructor(
        private route: ActivatedRoute,
        private catalogItemService: CatalogItemService
    ) {
    }

    ngOnInit() {
        const id = this.route.snapshot.paramMap.get('id');

        if (!id) {
            throw new Error('Catalog item id not found');
        }

        this._id = id;

        this.catalogItem$ = this.catalogItemService.getCatalogItemDetails(this._id);
        this.stockpiles$ = this.catalogItemService.getCatalogItemStockpiles(this._id);
    }

    handleSubmitDetails($event: ICatalogItemDetails) {
        this.catalogItemService.updateCatalogItemDetails($event)
            .subscribe(() => {
            });
    }

    handleSubmitAvailability($event: CatalogItemAvailability[]) {
        const availability: StockpileAvailability[] = [];

        for (let item of $event) {
            availability.push(new StockpileAvailability(item.stockpileId, item.available));
        }

        const req = new ChangeCatalogItemStockpileAvailabilityRequest(this._id!, availability);

        this.catalogItemService.changeCatalogItemStockpileAvailability(req)
            .subscribe();
    }
}
