/** @format */

import { Component, Input, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

export const CONTROL_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  // tslint:disable-next-line: no-use-before-declare
  useExisting: forwardRef(() => NoteComponent),
  multi: true,
};

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.scss'],
  providers: [CONTROL_VALUE_ACCESSOR],
})
export class NoteComponent implements ControlValueAccessor {
  @Input() closable: boolean;
  @Input() type: string;

  private _visible = true;

  private onTouchedCallback: () => void = () => {};
  private onChangeCallback: (_: any) => void = () => {};

  public get visible(): boolean {
    return this._visible;
  }

  public set visible(v: boolean) {
    if (v !== this._visible) {
      this._visible = v;
      this.onChangeCallback(v);
    }
  }

  public writeValue(v: boolean): void {
    if (v !== this._visible) {
      this._visible = v;
    }
  }

  public registerOnChange(fn: any): void {
    this.onChangeCallback = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouchedCallback = fn;
  }

  public onBlur() {
    this.onTouchedCallback();
  }
}
