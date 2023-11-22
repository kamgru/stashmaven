import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {PartnerDetailsComponent} from "./partners/partner-details/partner-details.component";

export const routes: Routes = [
  {path: '', redirectTo: 'partners', pathMatch: 'full'},
  {path: 'partner/:partnerId', component: PartnerDetailsComponent},
  {path: 'partners', component: PartnersComponent}
];
