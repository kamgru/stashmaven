import {Component, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnerPreviewComponent} from "./partner-preview/partner-preview.component";
import {RouterLink} from "@angular/router";
import {ListPartnersComponent} from "../common/list-partners/list-partners.component";
import {CreatePartnerComponent, ICreatedPartner} from "./create-partner/create-partner.component";
import {IPartner} from "../common/list-partners/list-partners.service";
import {PartnerService} from "../common/services/partner.service";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, ListPartnersComponent, PartnerPreviewComponent, RouterLink, CreatePartnerComponent],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {

    public uiState: 'list' | 'add' | 'edit' = 'list';
    public selectedPartner: IPartner | null = null;

    @ViewChild(ListPartnersComponent)
    private _listPartners?: ListPartnersComponent;

    constructor(
        private partnerService: PartnerService,
    ) {
    }

    handleAddPartner() {
        this.uiState = 'add';
    }

    handlePartnerSelected($event: IPartner) {
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
