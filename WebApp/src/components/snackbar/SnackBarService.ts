/** @format */

import { EventEmitter } from 'events';

export enum SnackBarType {
  DEFAULT,
  ERROR,
}

export interface SnackBarPayload {
  type: SnackBarType;
  content: string | JSX.Element;
}

export default class SnackBarService {
  public static events = new EventEmitter();

  public static show(
    content: string | JSX.Element,
    type: SnackBarType = SnackBarType.DEFAULT,
    timems: number = 3500
  ) {
    this.events.emit('show', { content, type } as SnackBarPayload);
    setTimeout(() => this.hide(), timems);
  }

  public static hide() {
    this.events.emit('hide');
  }
}
