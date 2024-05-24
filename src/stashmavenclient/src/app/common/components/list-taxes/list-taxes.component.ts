import {Component, OnDestroy} from '@angular/core';
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";
import * as ta from "./list-taxes.service";
import * as li from "../list-items";

@Component({
    selector: 'app-list-taxes',
    standalone: true,
    imports: [
        AsyncPipe,
        li.ListItemsLayoutComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-taxes.component.html',
})
export class ListTaxesComponent
    extends li.ListItemsBaseComponent<
        ta.ITaxDefinition,
        ta.ListTaxDefinitionsRequest,
        ta.IListTaxDefinitionsResponse,
        ta.ListTaxDefinitionsService>
    implements OnDestroy {

    constructor(
        listTaxDefinitions: ta.ListTaxDefinitionsService,
    ) {
        super();
        this.bootstrap(listTaxDefinitions);
    }
}
