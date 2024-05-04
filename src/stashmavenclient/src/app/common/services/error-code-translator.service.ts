import {Injectable} from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ErrorCodeTranslatorService {

    private _map = new Map<number, string>([
        [400000, 'Brand not found'],
        [400001, 'Brand short code is already in use'],
        [500000, 'Stockpile not found'],
        [500001, 'Stockpile has shipments'],
        [500002, 'Stockpile short code is not unique'],
        [500003, 'A default stockpile is required']
    ]);

    public translateErrorCode(code: number): string {
        return this._map.get(code) || 'Unknown error';
    }
}
