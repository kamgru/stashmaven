import {Component, Input} from '@angular/core';
import {IShipmentRecord} from "../../../../common/services/shipment.service";

@Component({
    selector: 'app-list-records',
    standalone: true,
    imports: [],
    templateUrl: './list-records.component.html',
    styleUrl: './list-records.component.css'
})
export class ListRecordsComponent {

    @Input()
    public records: IShipmentRecord[] = [];

    @Input()
    public currency: string = '';
}
