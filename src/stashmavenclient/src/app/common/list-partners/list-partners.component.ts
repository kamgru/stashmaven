import {Component, OnDestroy} from '@angular/core';
import {IListPartnersResponse, IPartner, ListPartnersRequest, ListPartnersService} from "./list-partners.service";
import {AsyncPipe} from "@angular/common";
import {ListItemsBaseComponent} from "../components/list-items";
import {ListItemsLayoutComponent} from "../components/list-items";

@Component({
    selector: 'app-list-partners',
    standalone: true,
    imports: [
        AsyncPipe,
        ListItemsLayoutComponent
    ],
    templateUrl: './list-partners.component.html',
    styleUrl: './list-partners.component.css'
})
export class ListPartnersComponent
    extends ListItemsBaseComponent<IPartner, ListPartnersRequest, IListPartnersResponse, ListPartnersService>
    implements OnDestroy {

    constructor(
        listPartners: ListPartnersService,
    ) {
        super();
        this.bootstrap(listPartners);
    }
}
