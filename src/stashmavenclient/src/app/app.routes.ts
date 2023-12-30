import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {CreatePartnerComponent} from "./partners/create-partner/create-partner.component";
import {EditPartnerComponent} from "./partners/edit-partner/edit-partner.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";
import {AddBrandComponent} from "./catalog/add-brand/add-brand.component";
import {ListBrandsComponent} from "./catalog/list-brands/list-brands.component";
import {ListInventoryItemsComponent} from "./inventory/list-inventory-items/list-inventory-items.component";
import {TaxDefinitionsComponent} from "./common/tax-definitions/tax-definitions.component";
import {CatalogItemsComponent} from "./catalog/catalog-items/catalog-items.component";
import {ShipmentsComponent} from "./inventory/shipments/shipments.component";

export const routes: Routes = [
    {path: 'partners/edit/:partnerId', component: EditPartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners/create', component: CreatePartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'catalog/catalog-items', component: CatalogItemsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/add', component: AddBrandComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/list', component: ListBrandsComponent, canActivate: [MsalGuard]},
    {path: 'common/tax-definitions', component: TaxDefinitionsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/items/list', component: ListInventoryItemsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments', component: ShipmentsComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent}
];
