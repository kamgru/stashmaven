import {Component, ElementRef, EventEmitter, HostListener, OnInit, Output, ViewChild} from '@angular/core';
import {CommonModule} from '@angular/common';
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";
import {
    IListPartnersResponse, IPartner,
    ListPartnersRequest,
    PartnersService
} from "../../../../../common/services/partners.service";

@Component({
    selector: 'app-select-shipment-partner',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './select-shipment-partner.component.html',
    styleUrls: ['./select-shipment-partner.component.css']
})
export class SelectShipmentPartnerComponent implements OnInit {

    search$ = new Subject<string>();
    kbdEvent$ = new Subject<KeyboardEvent>();

    currentIndex = 0;
    partners: IPartner[] = [];

    @ViewChild('searchInput') searchInput?: ElementRef;
    @Output() onPartnerSelected = new EventEmitter<IPartner>();

    @HostListener('window:keydown', ['$event'])
    handleKeyDown(event: KeyboardEvent) {
        this.kbdEvent$.next(event);
    }

    constructor(
        private partnersService: PartnersService,
    ) {
    }

    ngAfterViewInit() {
        this.searchInput?.nativeElement.focus();
    }

    ngOnInit() {
        this.search$
            .pipe(
                debounceTime(500),
                distinctUntilChanged(),
                switchMap((search: string) => {
                    const req = new ListPartnersRequest(1, 10, search, 'customIdentifier', true);
                    return this.partnersService.listPartners(req);
                })
            )
            .subscribe((res: IListPartnersResponse) => {
                this.partners = res.items;
                this.currentIndex = 0;
            });

        this.kbdEvent$
            .subscribe((event: KeyboardEvent) => {
                if (event.key == 'ArrowDown') {
                    event.preventDefault();
                    this.currentIndex = Math.min(this.currentIndex + 1, this.partners.length - 1);
                } else if (event.key == 'ArrowUp') {
                    event.preventDefault();
                    this.currentIndex = Math.max(this.currentIndex - 1, 0);
                } else if (event.key == 'Enter') {
                    event.preventDefault();
                    const partner = this.partners[this.currentIndex];
                    this.onPartnerSelected.emit(partner);
                }
            });
    }

}
