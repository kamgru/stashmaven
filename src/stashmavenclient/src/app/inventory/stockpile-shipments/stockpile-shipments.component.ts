import {Component, OnInit} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {ListShipmentsComponent} from "../../common/list-shipments/list-shipments.component";
import {forkJoin, map} from "rxjs";
import {StockpileService} from "../../common/services/stockpile.service";
import {IStockpileListItem} from "../../common/IStockpileListItem";
import {AddShipmentRequest, ShipmentService} from "../../common/services/shipment.service";
import {DropdownComponent, IDropdownItem} from "../../common/components/dropdown/dropdown.component";

@Component({
    selector: 'app-stockpile-shipments',
    standalone: true,
    imports: [
        AsyncPipe,
        ListShipmentsComponent,
        DropdownComponent
    ],
    templateUrl: './stockpile-shipments.component.html',
    styleUrl: './stockpile-shipments.component.css'
})
export class StockpileShipmentsComponent implements OnInit {

    private _selectedStockpile?: IStockpileListItem;

    public stockpiles: IStockpileListItem[] = [];
    public shipmentKinds$ = this.shipmentService.listShipmentKinds()
        .pipe(
            map(x => {
                return x.items.map(y => {
                    return <IDropdownItem>{
                        id: y.shipmentKindId,
                        name: y.name
                    };
                });
            })
        );

    constructor(
        private stockpileService: StockpileService,
        private shipmentService: ShipmentService) {
    }

    ngOnInit(): void {
        forkJoin([
            this.stockpileService.listStockpiles(),
            this.stockpileService.getDefaultStockpileId()
        ])
            .pipe(
                map(([stockpiles, defaultStockpile]) => {
                        return stockpiles.items.map(x => {
                            return {
                                stockpileId: x.stockpileId,
                                name: x.name,
                                shortCode: x.shortCode,
                                isDefault: x.stockpileId === defaultStockpile.stockpileId
                            };
                        });
                    }
                ))
            .subscribe(x => {
                this.stockpiles = x;
                this.stockpiles.sort((a, b) => a.isDefault ? -1 : b.isDefault ? 1 : 0);
                this._selectedStockpile = this.stockpiles[0];
            })
    }

    handleShipmentKindSelected($event: string) {
        console.log($event);
        this.shipmentService.addShipment(new AddShipmentRequest(this._selectedStockpile!.stockpileId, $event, 'Pln'))
            .subscribe(x => {
                console.log(x);
            })
    }

    handleStockpileChanged($event: IStockpileListItem) {
        this._selectedStockpile = $event;
    }
}
