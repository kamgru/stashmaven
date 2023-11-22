import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnersListComponent} from "./partners-list/partners-list.component";
import {Partner, PartnersService} from "./partners.service";
import {PartnerPreviewComponent} from "./partner-preview/partner-preview.component";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, PartnersListComponent, PartnerPreviewComponent],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {
    selectedPartner?: Partner | null;

    constructor(
        private partnersService: PartnersService
    ) {
    }

    ngOnInit() {
        this.partnersService.selectedPartner
            .subscribe((partner: Partner | null) => this.selectedPartner = partner)
    }
}
