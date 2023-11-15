import {Routes} from '@angular/router';
import {PartnersListComponent} from "./partners/partners-list/partners-list.component";

export const routes: Routes = [
  {path: '', redirectTo: 'partners', pathMatch: 'full'},
  {path: 'partners', component: PartnersListComponent}
];
