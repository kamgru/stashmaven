import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {PartnersService, ListPartnerRequest, PartnerList} from "./partners.service";

@Component({
  selector: 'app-partners',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './partners.component.html',
  styleUrls: ['./partners.component.css']
})
export class PartnersComponent {

  private request = {
    page: 0
  }

  partnerList: PartnerList | undefined;

  constructor(private partnersService: PartnersService) {
  }

  ngOnInit(): void {
    this.partnersService.listPartners(new ListPartnerRequest()).subscribe(data => this.partnerList = data);
  }

}
