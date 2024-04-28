import {Component} from '@angular/core';
import {ListBrandsComponent} from "../../common/components/list-brands/list-brands.component";
import {ActivatedRoute, Router, RouterLink} from "@angular/router";
import {AddBrandComponent, BrandAddedEvent} from "./add-brand/add-brand.component";
import {IBrandItem} from "../../common/components/list-brands/list-brands.service";
import {FaIconComponent, FaIconLibrary} from "@fortawesome/angular-fontawesome";
import {faPlus} from "@fortawesome/free-solid-svg-icons";
import {AddBrandRequest, BrandService} from "../../common/services/brand.service";

@Component({
    selector: 'app-brands',
    standalone: true,
    imports: [
        ListBrandsComponent,
        RouterLink,
        AddBrandComponent,
        FaIconComponent
    ],
    templateUrl: './brands.component.html',
})
export class BrandsComponent {

    public uiState: 'list' | 'add' | 'edit' = 'list';

    constructor(
        faLibrary: FaIconLibrary,
        private brandService: BrandService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        faLibrary.addIcons(faPlus);
    }

    handleItemConfirmed($event: IBrandItem) {
        this.router.navigate([$event.brandId], {relativeTo: this.route})
            .then(_ => {});
    }

    handleBrandAdded($event: BrandAddedEvent) {
        const req = new AddBrandRequest($event.name, $event.shortCode);
        this.brandService.addBrand(req).subscribe(() => {
            this.uiState = 'list';
        });

    }
}
