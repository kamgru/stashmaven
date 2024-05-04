import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {catchError, switchMap} from "rxjs";
import {BrandService, UpdateBrandRequest} from "../../../common/services/brand.service";
import {JsonPipe} from "@angular/common";
import {Notification, NotificationComponent} from "../../../common/components/notification/notification.component";
import {ErrorCodeTranslatorService} from "../../../common/services/error-code-translator.service";

@Component({
    selector: 'app-edit-brand',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        JsonPipe,
        NotificationComponent
    ],
    templateUrl: './edit-brand.component.html',
})
export class EditBrandComponent implements OnInit {

    private _brandId!: string;

    public notification: Notification | null = null;

    public editBrandForm = this.fb.group({
        brandId: ['', [Validators.required]],
        name: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
        shortCode: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(2)]],
    });

    public get brandId(): FormControl<string> {
        return this.editBrandForm.get('brandId') as FormControl<string>;
    }

    public get name(): FormControl<string> {
        return this.editBrandForm.get('name') as FormControl<string>;
    }

    public get shortCode(): FormControl<string> {
        return this.editBrandForm.get('shortCode') as FormControl<string>;
    }

    @ViewChild('nameInput', {static: true})
    public nameInput!: ElementRef<HTMLInputElement>;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private brandService: BrandService,
        private fb: FormBuilder
    ) {
    }

    public ngOnInit() {
        this.route.params.pipe(
            switchMap(params => {
                this._brandId = params['id'];
                return this.brandService.getBrand(this._brandId);
            })
        ).subscribe(brand => {
            this.brandId.setValue(this._brandId);
            this.name.setValue(brand.name);
            this.shortCode.setValue(brand.shortCode);
            this.nameInput.nativeElement.focus();
        });
    }

    public handleSubmit() {
        if (!this.editBrandForm.valid) {
            return;
        }

        this.brandService.updateBrand(
            new UpdateBrandRequest(
                this.brandId.value,
                this.name.value,
                this.shortCode.value))
            .pipe(
                catchError(err => {
                    this.notification = Notification.error(err.error);
                    return [];
                }))
            .subscribe(res => {
                this.notification = Notification.success('Brand updated successfully');
            });
    }

    handleCancelled() {
        this.router.navigate(['../'], {relativeTo: this.route})
            .then(() => {
            });
    }
}
