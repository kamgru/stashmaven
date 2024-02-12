import {Component, OnDestroy} from '@angular/core';
import {IListPartnersResponse, IPartner, ListPartnersRequest, ListPartnersService} from "./list-partners.service";
import {AsyncPipe} from "@angular/common";
import {ListItemsComponentBase} from "../components/list-items-component-base.component";

@Component({
    selector: 'app-list-partners',
    standalone: true,
    imports: [
        AsyncPipe
    ],
    templateUrl: './list-partners.component.html',
    styleUrl: './list-partners.component.css'
})
export class ListPartnersComponent
    extends ListItemsComponentBase<IPartner, ListPartnersRequest, IListPartnersResponse, ListPartnersService>
    implements OnDestroy {

    constructor(
        private listPartners: ListPartnersService,
    ) {
        super();
        this.bootstrap(listPartners);
        this.items$ = this.listPartners.items$;
    }
}
