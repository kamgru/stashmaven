import {Component, Input} from '@angular/core';
import {ReactiveFormsModule} from "@angular/forms";
import {IInventoryItem} from "../../../../common/components/list-inventory/list-inventory.service";

@Component({
    selector: 'app-edit-record',
    standalone: true,
    imports: [
        ReactiveFormsModule
    ],
    templateUrl: './edit-record.component.html'
})
export class EditRecordComponent {

    @Input({required: true})
    public inventoryItem: IInventoryItem | null = null;



}
