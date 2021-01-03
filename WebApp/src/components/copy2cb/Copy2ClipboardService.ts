/** @format */

import { EventEmitter } from 'events';

export default class Copy2ClipboardService {
  public static readonly events = new EventEmitter();

  public static copy(text: string) {
    this.events.emit('copy', text);
  }
}
