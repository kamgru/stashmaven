import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnersService, ListPartnerRequest, PartnerList, Partner} from "./partners.service";
import {TableLazyLoadEvent, TableModule} from "primeng/table";
import {AutoCompleteModule} from "primeng/autocomplete";

@Component({
  selector: 'app-partners',
  standalone: true,
  imports: [CommonModule, TableModule, AutoCompleteModule],
  templateUrl: './partners.component.html',
  styleUrls: ['./partners.component.css']
})
export class PartnersComponent {

  private request : ListPartnerRequest = {
    page: 1,
    pageSize: 33,
    isAscending: true,
    sortBy: "",
    search: "",
  }

  partnerList: PartnerList | undefined;
  selectedPartner: Partner | undefined;

  constructor(private partnersService: PartnersService) {
  }

  ngOnInit(): void {
    this.partnersService.listPartners(this.request)
      .subscribe(data => {
        console.log('dupa')
        this.partnerList = data
      });
  }

  loadMore($event: TableLazyLoadEvent) {
console.log($event)
    this.partnersService.listPartners(this.request)
      .subscribe(data => {
        this.partnerList = data
      });
  }
}
