/** @format */

import { Injectable, Inject } from '@angular/core';
import { IAPIProvider } from './api.provider';

/** @format */

@Injectable({
  providedIn: 'root',
})
export class APIService implements IAPIProvider {
  constructor(@Inject('APIProvider') private provider: IAPIProvider) {}

  public authLogin = this.provider.authLogin;
  public authLogout = this.provider.authLogout;
  public settingsGet = this.provider.settingsGet;
  public settingsSet = this.provider.settingsSet;
  public settingsInit = this.provider.settingsInit;
  public slGet = this.provider.slGet;
  public slGetSingle = this.provider.slGetSingle;
  public slCreate = this.provider.slCreate;
  public slEdit = this.provider.slEdit;
  public slDelete = this.provider.slDelete;
  public slSetPassword = this.provider.slSetPassword;
}
