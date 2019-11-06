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

  public authLogin = this.provider.authLogin.bind(this.provider);
  public authLogout = this.provider.authLogout.bind(this.provider);
  public settingsGet = this.provider.settingsGet.bind(this.provider);
  public settingsSet = this.provider.settingsSet.bind(this.provider);
  public settingsInit = this.provider.settingsInit.bind(this.provider);
  public slGet = this.provider.slGet.bind(this.provider);
  public slGetSingle = this.provider.slGetSingle.bind(this.provider);
  public slCreate = this.provider.slCreate.bind(this.provider);
  public slEdit = this.provider.slEdit.bind(this.provider);
  public slDelete = this.provider.slDelete.bind(this.provider);
  public slSetPassword = this.provider.slSetPassword.bind(this.provider);
}
