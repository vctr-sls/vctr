/** @format */

import { Injectable, Inject, EventEmitter } from '@angular/core';
import { IAPIProvider } from './api.provider';
import { Router } from '@angular/router';

/** @format */

@Injectable({
  providedIn: 'root',
})
export class APIService implements IAPIProvider {
  public authorizationError: EventEmitter<any>;

  constructor(
    @Inject('APIProvider') private provider: IAPIProvider,
    private router: Router
  ) {
    this.authorizationError = provider.authorizationError;
    this.authorizationError.subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

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
