import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnerPreviewComponent} from "./partner-preview/partner-preview.component";
import {RouterLink} from "@angular/router";
import {ListPartnersComponent} from "../common/list-partners/list-partners.component";
import {CreatePartnerComponent} from "./create-partner/create-partner.component";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, ListPartnersComponent, PartnerPreviewComponent, RouterLink, CreatePartnerComponent],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {

    public uiState: 'list' | 'edit' = 'list';

    handleAddPartner() {
        this.uiState = 'edit';
    }
}
