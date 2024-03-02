import {Component, OnDestroy} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import * as pa from "./list-partners.service";
import * as li from "../list-items";

@Component({
    selector: 'app-list-partners',
    standalone: true,
    imports: [
        AsyncPipe,
        li.ListItemsLayoutComponent
    ],
    templateUrl: './list-partners.component.html',
    styleUrl: './list-partners.component.css'
})
export class ListPartnersComponent
    extends li.ListItemsBaseComponent<pa.IPartner, pa.ListPartnersRequest, pa.IListPartnersResponse, pa.ListPartnersService>
    implements OnDestroy {

    constructor(
        listPartners: pa.ListPartnersService,
    ) {
        super();
        this.bootstrap(listPartners);
    }
}
