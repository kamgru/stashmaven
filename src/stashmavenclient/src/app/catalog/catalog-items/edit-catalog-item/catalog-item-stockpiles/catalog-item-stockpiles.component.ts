import {Component, EventEmitter, Input, Output} from "@angular/core";
import {ICatalogItemStockpile} from "../../../../common/services/catalog-item.service";
import {FormArray, FormBuilder, ReactiveFormsModule} from "@angular/forms";
import {JsonPipe} from "@angular/common";

export class CatalogItemAvailability {
    constructor(
        public stockpileId: string,
        public available: boolean
    ) {
    }
}

@Component({
    selector: 'app-catalog-item-stockpiles',
    standalone: true,
    templateUrl: './catalog-item-stockpiles.component.html',
    imports: [
        ReactiveFormsModule,
        JsonPipe
    ]
})
export class CatalogItemStockpilesComponent {

    private _initialState: CatalogItemAvailability[] = [];

    public stockpileForm = this.formBuilder.group({
        items: this.formBuilder.array([])
    })

    public get items() {
        return this.stockpileForm.get('items') as FormArray;
    }

    @Input()
    public set stockpiles(value: ICatalogItemStockpile[]) {
        this._initialState = value.map(x => new CatalogItemAvailability(x.stockpileId, x.quantity !== null));
        this.addFormItems(value);

        this.items.valueChanges.subscribe(_ => {
            this.refreshFormState();
        })
    }

    @Output()
    public OnFormSubmit = new EventEmitter<CatalogItemAvailability[]>

    constructor(
        private formBuilder: FormBuilder
    ) {
    }

    public handleSubmit() {
        if (!this.stockpileForm.valid) {
            throw new Error('Form not valid')
        }

        const items = this.items.controls.map(
            x => new CatalogItemAvailability(
                x.get('stockpileId')?.value ?? '',
                x.get('available')?.value
            ))

        this.OnFormSubmit.emit(items);
    }

    private addFormItems(value: ICatalogItemStockpile[]){
        this.items.clear();
        for (let stockpile of value) {
            const ctrl = this.formBuilder.group({
                stockpileId: [stockpile.stockpileId],
                available: [stockpile.quantity !== null],
                stockpileShortCode: [stockpile.stockpileShortCode],
                stockpileName: [stockpile.stockpileName],
                quantity: [stockpile.quantity],
            })
            this.items.push(ctrl);

            if (stockpile.quantity > 0) {
                ctrl.disable();
            }
        }
    }

    private refreshFormState(){
        for (let i = 0; i < this.items.length; i++) {
            const ctrl = this.items.at(i);
            if (ctrl.pristine){
                continue;
            }

            const initialState = this._initialState[i];
            if (ctrl.get('available')?.value === initialState.available) {
                ctrl.markAsPristine();
            }
        }
    }
}