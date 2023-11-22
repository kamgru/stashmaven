import {Component, Input, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {EditPartnerService} from "./edit-partner.service";

@Component({
    selector: 'app-edit-partner',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './edit-partner.component.html',
    styleUrls: ['./edit-partner.component.css']
})
export class EditPartnerComponent implements OnInit {

    @Input() partnerId: string | null = null;

    constructor(
        private editPartnerService: EditPartnerService) {
    }

    ngOnInit() {
        if (!this.partnerId){
            return;
        }

        this.editPartnerService.getPartner(this.partnerId)
            .subscribe(partner => {
                console.log(partner);
            })
    }
}
