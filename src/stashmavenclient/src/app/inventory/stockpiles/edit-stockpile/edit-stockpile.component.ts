import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {StockpileService, UpdateStockpileRequest} from "../../../common/services/stockpile.service";
import {catchError, throwError} from "rxjs";
import {Notification, NotificationComponent} from "../../../common/components/notification/notification.component";

@Component({
    selector: 'app-edit-stockpile',
    standalone: true,
    imports: [
        FormsModule,
        ReactiveFormsModule,
        NotificationComponent
    ],
    templateUrl: './edit-stockpile.component.html'
})
export class EditStockpileComponent implements OnInit {

    public notification: Notification | null = null;

    public editStockpileForm = this.fb.group({
        stockpileId: ['', [Validators.required]],
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
            .subscribe(stockpile => {
                this.editStockpileForm.get('stockpileId')?.setValue(stockpileId);
                this.name.setValue(stockpile.name);
                this.shortCode.setValue(stockpile.shortCode);
                this.isDefault.setValue(stockpile.isDefault);
                this.nameInput.nativeElement.focus();

                if (this.isDefault.value) {
                    this.isDefault.disable();
                }

                this.editStockpileForm.markAsPristine();
            });
    }

    public handleSubmit() {
        if (!this.editStockpileForm.valid) {
            return;
        }

        const stockpileId = this.editStockpileForm.get('stockpileId')?.value;

        const req = new UpdateStockpileRequest(
            this.name.value,
            this.shortCode.value,
            this.isDefault.value
        );

        this.stockpileService.updateStockpile(stockpileId!, req)
            .pipe(
                catchError(err => {
                    this.notification = Notification.error(err.error);
                    return [];
                })
            )
            .subscribe(() => {
                this.notification = Notification.success('Stockpile updated successfully');

                if (this.isDefault.value) {
                    this.isDefault.disable();
                }
                this.editStockpileForm.markAsPristine();
            });
    }

    handleCancelled() {
        this.router.navigate(['../'], {relativeTo: this.route});
    }
}
