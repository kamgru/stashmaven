import {Injectable} from '@angular/core';
import {Observable, of} from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class UnitOfMeasureService {

    constructor() {
    }

    public getUnitsOfMeasure(): Observable<string[]> {
        return of(['Pc', 'Kg', 'L']);
    }
}
