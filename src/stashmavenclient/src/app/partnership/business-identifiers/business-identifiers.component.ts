import {Component} from '@angular/core';
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {ActivatedRoute, Router} from "@angular/router";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {BusinessIdentifierService} from "../../common/services/business-identifier.service";
import {Notification, NotificationComponent} from "../../common/components/notification/notification.component";
import {AddStockpileComponent} from "../../inventory/stockpiles/add-stockpile/add-stockpile.component";
import {ListStockpilesComponent} from "../../common/components/list-stockpiles/list-stockpiles.component";
import {
    ListBusinessIdentifiersComponent
} from "../../common/components/list-business-identifiers/list-business-identifiers.component";
import {IBusinessIdentifier} from "../../common/components/list-business-identifiers/list-business-identifiers.service";
import {
    AddBusinessIdentifierComponent,
    BusinessIdentifierAddedEvent
} from "./add-business-identifier/add-business-identifier.component";

@Component({
    selector: 'app-business-identifiers',
    standalone: true,
    imports: [
        AddStockpileComponent,
        FaIconComponent,
        ListStockpilesComponent,
        NotificationComponent,
        ListBusinessIdentifiersComponent,
        AddBusinessIdentifierComponent
    ],
    templateUrl: './business-identifiers.component.html'
})
export class BusinessIdentifiersComponent {

    public uiState: 'list' | 'add' = 'list';

    public notification: Notification | null = null;

    constructor(
        fa: FaIconLibrary,
        private route: ActivatedRoute,
        private router: Router,
        private businessIdentifierService: BusinessIdentifierService
    ) {
        fa.addIcons(faPlus);
    }

    public handleItemConfirmed($event: IBusinessIdentifier) {
        this.router.navigate([$event.businessIdentifierId], {relativeTo: this.route});
    }

    public handleBusinessIdentifierAdded($event: BusinessIdentifierAddedEvent) {
        this.businessIdentifierService.addBusinessIdentifier($event)
            .subscribe(() => {
                this.uiState = 'list';
                this.notification = null;
            });
    }
}
