import {Routes} from '@angular/router';
import {PartnersComponent} from "./partnership/partners/partners.component";
import {MsalGuard, MsalRedirectComponent} from "@azure/msal-angular";
import {ProductsComponent} from "./catalog/products/products.component";
import {BrandsComponent} from "./catalog/brands/brands.component";
import {CountriesComponent} from "./common/countries/countries.component";
import {EditProductComponent} from "./catalog/products/edit-product/edit-product.component";
import {StockpilesComponent} from "./inventory/stockpiles/stockpiles.component";
import {ShipmentsComponent} from "./inventory/shipments/shipments.component";
import {OldEditShipmentComponent} from "./inventory/shipments/old-edit-shipment/old-edit-shipment.component";
import {EditBrandComponent} from "./catalog/brands/edit-brand/edit-brand.component";
import {EditStockpileComponent} from "./inventory/stockpiles/edit-stockpile/edit-stockpile.component";
import {InventoryItemsComponent} from "./inventory/inventory-items/inventory-items.component";
import {ShipmentKindsComponent} from "./inventory/shipment-kinds/shipment-kinds.component";
import {TaxesComponent} from "./common/taxes/taxes.component";

export const routes: Routes = [
    {path: 'catalog/products', component: ProductsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/products/:id', component: EditProductComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands', component: BrandsComponent, canActivate: [MsalGuard]},
    {path: 'catalog/brands/:id', component: EditBrandComponent, canActivate: [MsalGuard]},
    {path: 'common/taxes', component: TaxesComponent, canActivate: [MsalGuard]},
    {path: 'auth', component: MsalRedirectComponent},
    {path: 'inventory/stockpiles', component: StockpilesComponent, canActivate: [MsalGuard]},
    {path: 'inventory/stockpiles/:id', component: EditStockpileComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments', component: ShipmentsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipments/:shipmentId', component: OldEditShipmentComponent, canActivate: [MsalGuard]},
    {path: 'inventory/items', component: InventoryItemsComponent, canActivate: [MsalGuard]},
    {path: 'inventory/shipment-kinds', component: ShipmentKindsComponent, canActivate: [MsalGuard]},
    {path: 'partnership/partners', component: PartnersComponent, canActivate: [MsalGuard]},
    {path: 'countries', component: CountriesComponent, canActivate: [MsalGuard]}
];
