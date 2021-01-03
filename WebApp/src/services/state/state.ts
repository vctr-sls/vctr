/** @format */

import { EventEmitter } from 'events';
import { UserModel } from '../api/models';

export default class StateService {
  private _selfUser = (null as any) as UserModel;

  public readonly events = new EventEmitter();

  public get selfUser(): UserModel {
    return this._selfUser;
  }

  public set selfUser(u: UserModel) {
    this._selfUser = u;
    this.events.emit('update', u);
  }
}
