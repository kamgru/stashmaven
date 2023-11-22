import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {CreatePartnerComponent} from "./partners/create-partner/create-partner.component";
import {EditPartnerComponent} from "./partners/edit-partner/edit-partner.component";

export const routes: Routes = [
  {path: '', redirectTo: 'partners', pathMatch: 'full'},
  {path: 'partners/edit/:partnerId', component: EditPartnerComponent},
  {path: 'partners/create', component: CreatePartnerComponent},
  {path: 'partners', component: PartnersComponent}
];
