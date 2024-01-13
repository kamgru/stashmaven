import {Component, EventEmitter, HostListener, Input, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IInventoryItem} from "../../../../../common/services/stockpiles.service";

@Component({
    selector: 'app-input-details',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './input-details.component.html',
    styleUrls: ['./input-details.component.css']
})
export class InputDetailsComponent {
    private _inventoryItem?: IInventoryItem;

    public get inventoryItem(): IInventoryItem {
        return this._inventoryItem ?? {} as IInventoryItem;
    }
    @Input()
    public set inventoryItem(value: IInventoryItem) {
        this._inventoryItem = value;
    }

    @Output()
    public onInputDone = new EventEmitter<void>();


    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        if (event.key === 'Enter') {
            event.preventDefault();
            event.stopPropagation();
        }
        else if (event.key === 'Escape') {
            event.preventDefault();
            event.stopPropagation();
            this.onInputDone.emit();
        }
    }
}
