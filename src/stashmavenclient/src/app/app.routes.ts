import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {CreatePartnerComponent} from "./partners/create-partner/create-partner.component";
import {EditPartnerComponent} from "./partners/edit-partner/edit-partner.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";

export const routes: Routes = [
    {path: 'partners/edit/:partnerId', component: EditPartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners/create', component: CreatePartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent}
];
