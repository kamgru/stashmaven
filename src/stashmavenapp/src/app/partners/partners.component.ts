import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ListPartnerRequest, Partner, PartnerList, PartnersService} from "./partners.service";
import {TableLazyLoadEvent, TableModule} from "primeng/table";
import {AutoCompleteModule} from "primeng/autocomplete";
import {ChipsModule} from "primeng/chips";
import {debounceTime, distinctUntilChanged, Subject, switchMap} from "rxjs";

@Component({
  selector: 'app-partners',
  standalone: true,
  imports: [CommonModule, TableModule, AutoCompleteModule, ChipsModule],
  templateUrl: './partners.component.html',
  styleUrls: ['./partners.component.css']
})
export class PartnersComponent {
  partners: Partner[] = [];
  totalRecords: number = 0;
  rows: number = 10;

  private searchText$ = new Subject<string>();
  private listPartnerRequest: ListPartnerRequest = {
    page: 1,
    pageSize: 10,
    isAscending: true,
    sortBy: "",
    search: "",
  }

  constructor(private partnersService: PartnersService) {
  }

  ngOnInit() {
    this.searchText$.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap((searchText: string) => {
        this.listPartnerRequest.search = searchText;
        return this.partnersService.listPartners(this.listPartnerRequest);
      }))
      .subscribe((response: PartnerList) => {
        this.partners = response.partners;
        this.totalRecords = response.totalCount;
      });
  }

  loadPartnerLazy($event: TableLazyLoadEvent) {
    const first = $event.first ?? 0;
    const rows = $event.rows ?? 10;
    this.listPartnerRequest.page = first / rows + 1;
    this.listPartnerRequest.pageSize = rows;

    this.partnersService.listPartners(this.listPartnerRequest)
      .subscribe((response: PartnerList) => {
        this.partners = response.partners;
        this.totalRecords = response.totalCount;
      });
  }

  search($event: Event) {
    const searchPhrase = (($event.target) as HTMLInputElement).value;
    this.searchText$.next(searchPhrase);
  }
}
