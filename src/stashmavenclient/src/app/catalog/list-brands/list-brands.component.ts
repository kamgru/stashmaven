import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListBrandsRequest, ListBrandsService} from "./list-brands.service";
import {BehaviorSubject, Subject} from "rxjs";
import {SearchConfig, SuperTableConfig, SuperTableComponent} from "../../common/super-table/super-table.component";
import {Brand} from "../brand";

class ListBrandsState {
    constructor(
        public req: ListBrandsRequest,
        public firstLoadDone: boolean,
        public searchPhrase: string,
    ) {
    }
}

@Component({
    selector: 'app-list-brands',
    standalone: true,
    imports: [CommonModule, SuperTableComponent],
    templateUrl: './list-brands.component.html',
    styleUrls: ['./list-brands.component.css']
})
export class ListBrandsComponent implements OnInit {

    state: ListBrandsState = new ListBrandsState(
        new ListBrandsRequest(),
        false,
        '',
    );

    superTableConfig = new SuperTableConfig<Brand>(
        new SearchConfig(true, 'Search by brand name'),
        this.listBrandsService,
    );


    private state$ = new BehaviorSubject<ListBrandsState>(this.state);

    brands?: Brand[];

    constructor(
        public listBrandsService: ListBrandsService) {
    }

    ngOnInit(): void {
        // this.state$.subscribe((state: ListBrandsState) => {
        //     this.state = state;
        //     this.listBrandsService.listBrands(state.req).subscribe(data => {
        //         console.log('Brands loaded: ' + data)
        //     });
        // });

        // this.listBrandsService.listBrands(this.state.req).subscribe(data => {
        //     this.brands = data.brands;
        //
        // });
    }
}
