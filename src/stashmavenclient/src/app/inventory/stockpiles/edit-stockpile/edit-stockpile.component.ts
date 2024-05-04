import {Component, ElementRef, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {StockpileService} from "../../../common/services/stockpile.service";
import {catchError, throwError} from "rxjs";

@Component({
    selector: 'app-edit-stockpile',
    standalone: true,
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
    templateUrl: './edit-stockpile.component.html'
})
export class EditStockpileComponent {

    public editStockpileForm = this.fb.group({
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        shortCode: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(2)]],
        isDefault: [false]
    });

    public get name(): FormControl<string> {
        return this.editStockpileForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.editStockpileForm.get('shortCode') as FormControl<string>;
    }

    public get isDefault(): FormControl<boolean> {
        return this.editStockpileForm.get('isDefault') as FormControl<boolean>;
    }

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private stockpileService: StockpileService,
    ) {
    }

    public ngOnInit() {
        let stockpileId = this.route.snapshot.params['id'];

        this.stockpileService.getStockpile(stockpileId)
            .pipe(
                catchError(error => {
                    this.router.navigate(['/inventory/stockpiles']);
                    return throwError(error);
                })
            )
            .subscribe(stockpile => {
                this.name.setValue(stockpile.name);
                this.shortCode.setValue(stockpile.shortCode);
                this.isDefault.setValue(stockpile.isDefault);
                this.nameInput.nativeElement.focus();
            });

    }

    public handleSubmit() {
        if (!this.editStockpileForm.valid) {
            return;
        }
    }

    handleCancelled() {

    }
}
