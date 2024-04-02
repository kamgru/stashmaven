import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {map, Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export class AddCatalogItemRequest {
    constructor(
        public name: string,
        public sku: string,
        public unitOfMeasure: string,
    ) {
    }
}

export interface ICatalogItemDetails {
    catalogItemId: string;
    name: string;
    sku: string;
    unitOfMeasure: string;
}

export interface IGetCatalogItemStockpilesResponse {
    stockpiles: ICatalogItemStockpile[];
}

export interface ICatalogItemStockpile {
    stockpileId: string;
    stockpileShortCode: string;
    stockpileName: string;
    quantity: number;
}

export class AddInventoryItemRequest {
    constructor(
        public catalogItemId: string,
        public stockpileId: string
    ) {
    }
}

export class ChangeCatalogItemStockpileAvailabilityRequest {
    constructor(
        public catalogItemId: string,
        public stockpileAvailabilities: StockpileAvailability[]
    ) {
    }
}

export class StockpileAvailability {
    constructor(
        public stockpileId: string,
        public isAvailable: boolean
    ) {
    }
}

@Injectable({
    providedIn: 'root'
})
export class CatalogItemService {

    constructor(
        private http: HttpClient,
    ) {
    }

    public getCatalogItemDetails(catalogItemId: string): Observable<ICatalogItemDetails> {
        return this.http.get<ICatalogItemDetails>(`${environment.apiUrl}/api/v1/catalogitem/${catalogItemId}`).pipe(
            map((res: any) => {
                return {
                    catalogItemId: catalogItemId,
                    name: res.name,
                    sku: res.sku,
                    unitOfMeasure: res.unitOfMeasure
                };
            }));
    }

    public updateCatalogItemDetails(details: ICatalogItemDetails): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/catalogitem`, details);
    }

    public getCatalogItemStockpiles(catalogItemId: string): Observable<IGetCatalogItemStockpilesResponse> {
        return this.http.get<IGetCatalogItemStockpilesResponse>(
            `${environment.apiUrl}/api/v1/catalogitem/${catalogItemId}/stockpiles`);
    }

    public changeCatalogItemStockpileAvailability(request: ChangeCatalogItemStockpileAvailabilityRequest): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/inventoryitem/availability`, request);
    }

    add(req: AddCatalogItemRequest): Observable<string> {
        return this.http.post<string>(`${environment.apiUrl}/api/v1/catalogitem`, req);
    }
}