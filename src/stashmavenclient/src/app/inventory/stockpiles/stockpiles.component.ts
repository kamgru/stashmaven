import {Component} from '@angular/core';
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {ListStockpilesComponent} from "../../common/components/list-stockpiles/list-stockpiles.component";
import {IStockpile} from "../../common/components/list-stockpiles/list-stockpiles.service";
import {AddStockpileComponent, StockpileAddedEvent} from "./add-stockpile/add-stockpile.component";
import {AddStockpileRequest, StockpileService} from "../../common/services/stockpile.service";
import {catchError} from "rxjs";
import {NotificationComponent, Notification} from "../../common/components/notification/notification.component";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
    selector: 'app-stockpiles',
    standalone: true,
    imports: [
        FaIconComponent,
        ListStockpilesComponent,
        AddStockpileComponent,
        NotificationComponent
    ],
    templateUrl: './stockpiles.component.html'
})
export class StockpilesComponent {

    public uiState: 'list' | 'add' = 'list';

    public notification: Notification | null = null;

    constructor(
        fa: FaIconLibrary,
        private route: ActivatedRoute,
        private router: Router,
        private stockpileService: StockpileService
    ) {
        fa.addIcons(faPlus);
    }

    handleItemConfirmed($event: IStockpile) {
        this.router.navigate([$event.stockpileId], {relativeTo: this.route});
    }

    handleStockpileAdded($event: StockpileAddedEvent) {
        const req = new AddStockpileRequest($event.name, $event.shortCode, $event.isDefault);
        this.stockpileService.addStockpile(req)
            .pipe(
                catchError(err => {
                    this.notification = Notification.error(err.error);
                    return [];
                })
            ).subscribe(() => {
            this.uiState = 'list';
            this.notification = null;
        });
    }
}
