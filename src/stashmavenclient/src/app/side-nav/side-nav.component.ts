import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink, RouterLinkActive} from "@angular/router";
import {CompoundNavItemComponent} from "./nav-item/compound-nav-item.component";
import {SubNavItemComponent} from "./nav-item/nav-item-child/sub-nav-item.component";

@Component({
    selector: 'app-side-nav',
    standalone: true,
    imports: [CommonModule, RouterLink, RouterLinkActive, CompoundNavItemComponent, SubNavItemComponent],
    templateUrl: './side-nav.component.html',
})
export class SideNavComponent {

}
