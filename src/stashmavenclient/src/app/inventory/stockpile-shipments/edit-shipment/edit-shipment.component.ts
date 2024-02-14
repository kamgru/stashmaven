import {Component} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {IShipmentEditDetails, ShipmentService} from "../../../common/services/shipment.service";
import {switchMap} from "rxjs";
import {JsonPipe} from "@angular/common";
import {ListPartnersComponent} from "../../../common/list-partners/list-partners.component";
import {IPartner} from "../../../common/services/partners.service";

@Component({
    selector: 'app-edit-shipment',
    standalone: true,
    imports: [
        JsonPipe,
        ListPartnersComponent
    ],
    templateUrl: './edit-shipment.component.html',
    styleUrl: './edit-shipment.component.css'
})
export class EditShipmentComponent {

    public shipment?: IShipmentEditDetails;
    public uiState: 'view' | 'edit-partner' = 'view';

    constructor(
        private route: ActivatedRoute,
        private shipmentService: ShipmentService
    ) {
    }

    ngOnInit(): void {
        const id = this.route.snapshot.paramMap.get('shipmentId');
        if (id) {
            this.shipmentService.getShipment(id)
                .subscribe(x => {
                    this.shipment = x;
                });
        }
    }

    handlePartnerSelected($event: IPartner) {
        
    }
}
