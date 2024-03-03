import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {AddPartnerComponent} from "./partners/add-partner/add-partner.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";
import {AddBrandComponent} from "./catalog/add-brand/add-brand.component";
import {TaxDefinitionsComponent} from "./common/tax-definitions/tax-definitions.component";
import {CatalogItemsComponent} from "./catalog/catalog-items/catalog-items.component";
import {StockpileInventoryComponent} from "./inventory/stockpile-inventory/stockpile-inventory.component";
import {StockpileShipmentsComponent} from "./inventory/stockpile-shipments/stockpile-shipments.component";
import {BrandsComponent} from "./catalog/brands/brands.component";
import {EditShipmentComponent} from "./inventory/stockpile-shipments/edit-shipment/edit-shipment.component";

export const routes: Routes = [
    {path: 'partners/create', component: AddPartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'catalog/catalog-items', component: CatalogItemsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands', component: BrandsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/add', component: AddBrandComponent, canActivate: [MsalGuard]},
    {path: 'common/tax-definitions', component: TaxDefinitionsComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent},
    {path: 'inventory/stockpile', component: StockpileInventoryComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments', component: StockpileShipmentsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments/:shipmentId', component: EditShipmentComponent, canActivate: [MsalGuard]}
];
