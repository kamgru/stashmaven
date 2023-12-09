import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListBrandsRequest, ListBrandsService} from "./list-brands.service";
import {BehaviorSubject, Subject} from "rxjs";

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
    imports: [CommonModule],
    templateUrl: './list-brands.component.html',
    styleUrls: ['./list-brands.component.css']
})
export class ListBrandsComponent implements OnInit {

    state: ListBrandsState = new ListBrandsState(
        new ListBrandsRequest(),
        false,
        '',
    );

    private state$ = new BehaviorSubject<ListBrandsState>(this.state);

    constructor(
        private listBrandsService: ListBrandsService
    ) {
    }

    ngOnInit(): void {
        this.state$.subscribe((state: ListBrandsState) => {
            this.state = state;
            this.listBrandsService.listBrands(state.req).subscribe(data => {
                console.log('Brands loaded: ' + data)
            });
        });
    }
}
