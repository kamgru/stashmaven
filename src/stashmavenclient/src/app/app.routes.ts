import {Routes} from '@angular/router';
import {PartnersComponent} from "./partners/partners.component";
import {AddPartnerComponent} from "./partners/add-partner/add-partner.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";
import {AddBrandComponent} from "./catalog/add-brand/add-brand.component";
import {TaxDefinitionsComponent} from "./common/tax-definitions/tax-definitions.component";
import {ProductsComponent} from "./catalog/products/products.component";
import {StockpileInventoryComponent} from "./inventory/stockpile-inventory/stockpile-inventory.component";
import {BrandsComponent} from "./catalog/brands/brands.component";
import {CountriesComponent} from "./common/countries/countries.component";
import {EditProductComponent} from "./catalog/products/edit-product/edit-product.component";
import {StockpilesComponent} from "./inventory/stockpiles/stockpiles.component";
import {ShipmentsComponent} from "./inventory/shipments/shipments.component";
import {EditShipmentComponent} from "./inventory/shipments/edit-shipment/edit-shipment.component";

export const routes: Routes = [
    {path: 'partners/create', component: AddPartnerComponent, canActivate: [MsalGuard]},
    {path: 'partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'catalog/products', component: ProductsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/products/:id', component: EditProductComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands', component: BrandsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/add', component: AddBrandComponent, canActivate: [MsalGuard]},
    {path: 'common/tax-definitions', component: TaxDefinitionsComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent},
    {path: 'inventory/stockpile', component: StockpileInventoryComponent, canActivate: [MsalGuard]},
    {path: 'inventory/stockpiles', component: StockpilesComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments', component: ShipmentsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments/:shipmentId', component: EditShipmentComponent, canActivate: [MsalGuard]},
    {path: 'countries', component: CountriesComponent, canActivate: [MsalGuard]}
];
