import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnerPreviewComponent} from "./partner-preview/partner-preview.component";
import {RouterLink} from "@angular/router";
import {SelectPartnerService} from "./select-partner.service";
import {ListPartnersComponent} from "../common/list-partners/list-partners.component";

@Component({
    selector: 'app-partners',
    standalone: true,
    imports: [CommonModule, ListPartnersComponent, PartnerPreviewComponent, RouterLink],
    templateUrl: './partners.component.html',
    styleUrls: ['./partners.component.css']
})
export class PartnersComponent {
}
