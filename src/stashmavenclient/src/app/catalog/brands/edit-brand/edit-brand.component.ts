import {Component, ElementRef, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import {switchMap} from "rxjs";
import {BrandService, UpdateBrandRequest} from "../../../common/services/brand.service";
import {JsonPipe} from "@angular/common";

@Component({
    selector: 'app-edit-brand',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        JsonPipe
    ],
    templateUrl: './edit-brand.component.html',
})
export class EditBrandComponent implements OnInit {

    private _brandId!: string;

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
                this.shortCode.value
            )
        )
            .subscribe(res => {

            });
    }

    handleCancelled() {
        this.router.navigate(['../'], {relativeTo: this.route})
            .then(() => {});
    }
}
