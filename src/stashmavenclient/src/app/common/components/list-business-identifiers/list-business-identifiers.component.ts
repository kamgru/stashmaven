import {Component} from '@angular/core';
import * as ls from "./list-business-identifiers.service";
import {ListItemsBaseComponent, ListItemsLayoutComponent} from "../list-items";
import {AsyncPipe, NgTemplateOutlet} from "@angular/common";

@Component({
    selector: 'app-list-business-identifiers',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent,
        NgTemplateOutlet
    ],
    templateUrl: './list-business-identifiers.component.html'
})
export class ListBusinessIdentifiersComponent
    extends ListItemsBaseComponent<ls.IBusinessIdentifier, ls.ListBusinessIdentifiersRequest, ls.IListBusinessIdentifiersResponse, ls.ListBusinessIdentifiersService> {

    constructor(
        listBusinessIdentifiers: ls.ListBusinessIdentifiersService,
    ) {
        super();
        this.bootstrap(listBusinessIdentifiers);
    }
}
