import {Component, ElementRef, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {debounceTime, distinctUntilChanged, Subject} from "rxjs";

@Component({
    selector: 'app-list-search-input',
    standalone: true,
    imports: [],
    templateUrl: './list-search-input.component.html',
    styleUrl: './list-search-input.component.css'
})
export class ListSearchInputComponent {

    private _search$ = new Subject<string>();

    @ViewChild('searchInput', {static: true})
    private _searchInput?: ElementRef<HTMLInputElement>;

    @Input({required: true})
    public search: string = '';

    @Output()
    public OnSearch: EventEmitter<string> = new EventEmitter<string>();

    public focus() {
        this._searchInput?.nativeElement.focus();
    }

    public blur(){
        this._searchInput?.nativeElement.blur();
    }

    constructor() {
        this._search$
            .pipe(
                debounceTime(300),
                distinctUntilChanged()
            )
            .subscribe(x => this.OnSearch.emit(x));
    }

    handleInput(value: string) {
        this._search$.next(value);
    }
}
