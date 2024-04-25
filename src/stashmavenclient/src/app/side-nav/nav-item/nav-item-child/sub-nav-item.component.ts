import {Component, Input} from '@angular/core';
import {RouterLink, RouterLinkActive} from "@angular/router";

@Component({
    selector: 'app-nav-item-child',
    standalone: true,
    imports: [
        RouterLink,
        RouterLinkActive
    ],
    templateUrl: './sub-nav-item.component.html'
})
export class SubNavItemComponent {

    @Input({required: true})
    public name!: string;

    @Input({required: true})
    public routerLink!: string | string[];
}
