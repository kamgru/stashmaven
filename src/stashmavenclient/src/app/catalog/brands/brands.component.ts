import {Component} from '@angular/core';
import {ListBrandsComponent} from "../../common/components/list-brands/list-brands.component";
import {RouterLink} from "@angular/router";

@Component({
    selector: 'app-brands',
    standalone: true,
    imports: [
        ListBrandsComponent,
        RouterLink
    ],
    templateUrl: './brands.component.html',
    styleUrl: './brands.component.css'
})
export class BrandsComponent {

    public uiState: 'list' | 'edit' = 'list';
}
