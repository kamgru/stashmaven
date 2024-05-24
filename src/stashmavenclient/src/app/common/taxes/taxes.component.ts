import {Component} from '@angular/core';
import {AddStockpileComponent} from "../../inventory/stockpiles/add-stockpile/add-stockpile.component";
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {ListStockpilesComponent} from "../components/list-stockpiles/list-stockpiles.component";
import {Notification, NotificationComponent} from "../components/notification/notification.component";
import {ListTaxesComponent} from "../components/list-taxes/list-taxes.component";
import {ITaxDefinition} from "../components/list-taxes/list-taxes.service";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddTaxComponent, TaxDefinitionAddedEvent} from "./add-tax/add-tax.component";
import {AddTaxDefinitionRequest, TaxDefinitionService} from "../services/tax-definition.service";

@Component({
    selector: 'app-taxes',
    standalone: true,
    imports: [
        AddStockpileComponent,
        FaIconComponent,
        ListStockpilesComponent,
        NotificationComponent,
        ListTaxesComponent,
        AddTaxComponent
    ],
    templateUrl: './taxes.component.html'
})
export class TaxesComponent {

    public uiState: 'list' | 'add' = 'list';

    public notification: Notification | null = null;

    constructor(
        private taxDefinitionService: TaxDefinitionService,
        fa: FaIconLibrary
    ) {
        fa.addIcons(faPlus);
    }

    public handleItemConfirmed($event: ITaxDefinition) {

    }

    public handleTaxDefinitionAdded($event: TaxDefinitionAddedEvent) {
        const req = new AddTaxDefinitionRequest($event.name, $event.rate);
        this.taxDefinitionService.addTaxDefinition(req)
            .subscribe(() => {
                this.uiState = 'list';
            });
    }

}
