import {HttpClient} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {map, Observable} from "rxjs";
import {environment} from "../../../environments/environment";

export class AddProductRequest {
    constructor(
        public name: string,
        public sku: string,
        public unitOfMeasure: string,
    ) {
    }
}

export interface IAddProductResponse {
    productId: string;
}

export interface IGetProductDetailsResponse {
    productId: string;
    name: string;
    sku: string;
    unitOfMeasure: string;
}

export interface IGetProductStockpilesResponse {
    stockpiles: IProductStockpile[];
}

export interface IProductStockpile {
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

export class ChangeProductStockpileAvailabilityRequest {
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
export class ProductService {

    constructor(
        private http: HttpClient,
    ) {
    }

    public getProductDetails(productId: string): Observable<IGetProductDetailsResponse> {
        return this.http.get<IGetProductDetailsResponse>(`${environment.apiUrl}/api/v1/product/${productId}`).pipe(
            map((res: any) => {
                return {
                    productId: productId,
                    name: res.name,
                    sku: res.sku,
                    unitOfMeasure: res.unitOfMeasure
                };
            }));
    }

    public updateProductDetails(details: IGetProductDetailsResponse): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/catalogitem`, details);
    }

    public getProductStockpiles(productId: string): Observable<IGetProductStockpilesResponse> {
        return this.http.get<IGetProductStockpilesResponse>(
            `${environment.apiUrl}/api/v1/product/${productId}/stockpiles`);
    }

    public changeProductStockpileAvailability(request: ChangeProductStockpileAvailabilityRequest): Observable<void> {
        return this.http.patch<void>(`${environment.apiUrl}/api/v1/inventoryitem/availability`, request);
    }

    add(req: AddProductRequest): Observable<IAddProductResponse> {
        return this.http.post<IAddProductResponse>(`${environment.apiUrl}/api/v1/product`, req);
    }
}