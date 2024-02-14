import {signal} from "@angular/core";

export class Cursor {
    private _direction: 'up' | 'down' = 'down';
    private _items: number = 0;

    public index = signal(0);

    public tryMoveDown(): boolean {
        this._direction = 'down';
        if (this.canMoveDown()) {
            this.index.set(this.index() + 1);
            return true;
        }
        return false;
    }

    public tryMoveUp(): boolean {
        this._direction = 'up';
        if (this.canMoveUp()) {
            this.index.set(this.index() - 1);
            return true;
        }
        return false;
    }

    public tryReset(items: number): boolean {
        if (this.index() > items) {
            this.moveToTop(items);
            return true;
        }

        if (!(this.isAtBottom() || this.isAtTop())) {
            return false;
        }

        if (this._direction === 'down') {
            this.moveToTop(items);
        } else {
            this.moveToBottom(items);
        }

        return true;
    }

    public trySetIndex(index: number): boolean {
        if (index < 0 || index > this._items) {
            return false;
        }

        this.index.set(index);
        return true;
    }

    private moveToTop(items: number) {
        this.index.set(0);
        this._direction = 'down';
        this._items = items;
    }

    private moveToBottom(items: number) {
        this.index.set(items - 1);
        this._direction = 'up';
        this._items = items;
    }

    private canMoveDown(): boolean {
        return this.index() < this._items - 1;
    }

    private canMoveUp(): boolean {
        return this.index() > 0;
    }

    private isAtTop(): boolean {
        return this.index() == 0;
    }

    private isAtBottom(): boolean {
        return this.index() == this._items - 1;
    }
}