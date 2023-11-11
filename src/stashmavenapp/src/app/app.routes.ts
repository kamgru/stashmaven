import { Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {MsalGuard} from "@azure/msal-angular";
import {PartnersComponent} from "./partners/partners.component";

export const routes: Routes = [
  {path: 'home', component: HomeComponent, canActivate: [MsalGuard]},
  {path: 'partners', component: PartnersComponent, /*canActivate: [MsalGuard]*/},
];
