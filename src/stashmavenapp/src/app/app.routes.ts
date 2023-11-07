import { Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {MsalGuard} from "@azure/msal-angular";

export const routes: Routes = [
  {path: 'home', component: HomeComponent, canActivate: [MsalGuard]},
];
