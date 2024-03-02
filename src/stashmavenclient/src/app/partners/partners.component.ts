import {Component, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink} from "@angular/router";
import {CreatePartnerComponent, ICreatedPartner} from "./create-partner/create-partner.component";
import {PartnerService} from "../common/services/partner.service";
import * as p from "../common/components/list-partners";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, p.ListPartnersComponent, RouterLink, CreatePartnerComponent],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {

    public uiState: 'list' | 'add' | 'edit' = 'list';
    public selectedPartner: p.IPartner | null = null;

    @ViewChild(p.ListPartnersComponent)
    private _listPartners?: p.ListPartnersComponent;

    constructor(
        private partnerService: PartnerService,
    ) {
    }

    handleAddPartner() {
        this.uiState = 'add';
    }

    handlePartnerSelected($event: p.IPartner) {
        this.selectedPartner = $event;
    }

    handleDeletePartner() {
        this.partnerService.deletePartner(this.selectedPartner!.partnerId).subscribe(() => {
            this.uiState = 'list';
            this._listPartners?.reload();
        });
    }

    handlePartnerCreated($event: ICreatedPartner) {
        this.uiState = 'list';
        this._listPartners?.reload();
    }
}
