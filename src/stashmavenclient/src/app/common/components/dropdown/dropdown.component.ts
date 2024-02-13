import {Component, EventEmitter, Input, Output, TemplateRef} from '@angular/core';
import {NgTemplateOutlet} from "@angular/common";

export interface IDropdownItem {
    id: string;
    name: string;
}

@Component({
    selector: 'app-dropdown',
    standalone: true,
    imports: [
        NgTemplateOutlet
    ],
    templateUrl: './dropdown.component.html',
    styleUrl: './dropdown.component.css'
})
export class DropdownComponent {

    public isOpen = false;

    @Input()
    public items: IDropdownItem[] = [];

    @Input()
    public title: TemplateRef<any> | null = null;

    @Output()
    public OnItemSelected = new EventEmitter<string>();

    handleSelect(id: string) {
       this.OnItemSelected.emit(id);
       this.isOpen = false;
    }
}
