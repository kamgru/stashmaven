import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListPartnersComponent} from "./list-partners/list-partners.component";
import {PartnerPreviewComponent} from "./partner-preview/partner-preview.component";
import {RouterLink} from "@angular/router";
import {Partner} from "./partner";
import {SelectPartnerService} from "./select-partner.service";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, ListPartnersComponent, PartnerPreviewComponent, RouterLink],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {
    selectedPartner?: Partner | null;

    constructor(
        private partnersService: SelectPartnerService
    ) {
    }

    ngOnInit() {
        this.partnersService.selectedPartner
            .subscribe((partner: Partner | null) => this.selectedPartner = partner)
    }
}
