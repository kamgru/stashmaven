import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {CreatePartnerComponent} from "./partners/create-partner/create-partner.component";
import {EditPartnerComponent} from "./partners/edit-partner/edit-partner.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";
import {AddBrandComponent} from "./catalog/add-brand/add-brand.component";
import {TaxDefinitionsComponent} from "./common/tax-definitions/tax-definitions.component";
import {CatalogItemsComponent} from "./catalog/catalog-items/catalog-items.component";
import {StockpileInventoryComponent} from "./inventory/stockpile-inventory/stockpile-inventory.component";
import {StockpileShipmentsComponent} from "./inventory/stockpile-shipments/stockpile-shipments.component";

export const routes: Routes = [
    {path: 'partners/edit/:partnerId', component: EditPartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners/create', component: CreatePartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'catalog/catalog-items', component: CatalogItemsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/add', component: AddBrandComponent, canActivate: [MsalGuard]},
    {path: 'common/tax-definitions', component: TaxDefinitionsComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent},
    {path: 'inventory/stockpile', component: StockpileInventoryComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments', component: StockpileShipmentsComponent, canActivate: [MsalGuard]}
];
