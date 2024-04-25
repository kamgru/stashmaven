import {Component, Input, OnInit} from '@angular/core';
import {RouterLink} from "@angular/router";

@Component({
    selector: 'app-nav-item',
    standalone: true,
    imports: [
        RouterLink
    ],
    templateUrl: './compound-nav-item.component.html'
})
export class CompoundNavItemComponent implements OnInit {

    public navId!: string;

    @Input({required: true})
    public name!: string;

    @Input({required: true})
    public routerLink!: string | string[];


    public ngOnInit() {
        this.navId = encodeURIComponent(this.name.toLowerCase());
    }
}
