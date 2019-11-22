/** @format */

import {
  GeneralSettings,
  GeneralSettingsPost,
  ShortLink,
  SetPasswordPost,
  ProtectedLogin,
  ProtectedResponse,
  Size,
} from './api.models';
import { Observable } from 'rxjs';
import { EventEmitter } from '@angular/core';

/** @format */

export interface IAPIProvider {
  // Events
  error: EventEmitter<any>;
  authorizationError: EventEmitter<any>;

  // Protected Access
  protectedRedirect(
    guid: string,
    login: ProtectedLogin
  ): Promise<ProtectedResponse>;

  // Authorization
  authLogin(password: string): Promise<any>;
  authLogout(): Promise<any>;

  // General Settings
  settingsGet(): Observable<GeneralSettings>;
  settingsSet(settings: GeneralSettingsPost): Promise<GeneralSettings>;
  settingsInit(password: string): Promise<any>;

  // Short Links
  slGet(page: number, size: number, sortBy: string): Observable<ShortLink[]>;
  slGetSize(): Observable<Size>;
  slSearch(
    query: string,
    page: number,
    size: number,
    sortBy: string
  ): Observable<ShortLink[]>;
  slGetSingle(guid: string): Observable<ShortLink>;
  slCreate(shortLink: ShortLink): Promise<ShortLink>;
  slEdit(shortLink: ShortLink): Promise<ShortLink>;
  slDelete(guid: string): Promise<any>;
  slSetPassword(guid: string, password: SetPasswordPost): Promise<any>;
}
