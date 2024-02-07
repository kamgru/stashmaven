import {Component, EventEmitter, Input, Output} from '@angular/core';

export interface IDropdownItem {
    id: string;
    name: string;
}

@Component({
    selector: 'app-dropdown',
    standalone: true,
    imports: [],
    templateUrl: './dropdown.component.html',
    styleUrl: './dropdown.component.css'
})
export class DropdownComponent {

    public isOpen = false;

    @Input()
    public items: IDropdownItem[] = [];

    @Output()
    public OnItemSelected = new EventEmitter<string>();

    handleSelect(id: string) {
       this.OnItemSelected.emit(id);
       this.isOpen = false;
    }
}
