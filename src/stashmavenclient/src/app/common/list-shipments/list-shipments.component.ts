import {Component, EventEmitter, Input, OnDestroy, OnInit, Output, TemplateRef} from '@angular/core';
import {IShipment, ListShipmentsService} from "./list-shipments.service";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";
import * as li from "../components/list-items";
import * as ls from "./list-shipments.service";
import {IStockpileListItem} from "../IStockpileListItem";
import {ISelectOption, SelectComponent} from "../components/select/select.component";
import {tap} from "rxjs";
import {ListItemsLayoutComponent} from "../components/list-items";

@Component({
    selector: 'app-list-shipments',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent,
        SelectComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-shipments.component.html',
    styleUrl: './list-shipments.component.css'
})
export class ListShipmentsComponent
    extends li.ListItemsBaseComponent<IShipment, ls.ListShipmentsRequest, ls.IListShipmentsResponse, ListShipmentsService>
    implements OnInit, OnDestroy {

    @Input()
    public stockpiles: IStockpileListItem[] = [];

    @Output()
    public OnStockpileChanged = new EventEmitter<IStockpileListItem>();

    public selectOptions: ISelectOption[] = [];
    public selectedOption: ISelectOption | null = null;

    @Input()
    public controlsTemplate: TemplateRef<any> | null = null;

    constructor(
        private listShipments: ls.ListShipmentsService,
    ) {
        super();
        this.bootstrap(listShipments);

        this.listResponse$ = this.listShipments.items$
            .pipe(
                tap(x => {
                    const selectedStockpile = this.stockpiles.find(y => y.stockpileId === x.stockpileId);
                    this.selectedOption = {value: x.stockpileId, label: selectedStockpile?.name ?? ''};
                })
            );
    }

    ngOnInit() {
        this.selectOptions = this.stockpiles.map(x => ({value: x.stockpileId, label: x.name}));
    }

    handleStockpileChanged(value: ISelectOption) {
        const stockpile = this.stockpiles.find(x => x.stockpileId === value.value);
        this.listShipments.changeStockpile(stockpile?.stockpileId ?? '');
        this.OnStockpileChanged.emit(stockpile);
    }


}