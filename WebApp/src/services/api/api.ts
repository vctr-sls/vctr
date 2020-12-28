/** @format */

import { EventEmitter } from 'events';
import {
  LinkCreateModel,
  LinkModel,
  LoginModel,
  UserCreateModel,
  UserModel,
} from './models';

const PREFIX =
  process.env.NODE_ENV === 'development' ? 'http://localhost:5000/api' : '/api';

export class AuthenticationError extends Error {
  constructor() {
    super('authentication error');
  }
}

export default class APIService {
  public static readonly events = new EventEmitter();

  // ------------------------------------------------------------
  // --- AUTH ---

  public static authLogin(model: LoginModel): Promise<UserModel> {
    return this.post('auth/login', model);
  }

  public static authLogout(): Promise<any> {
    return this.post('auth/logout');
  }

  // ------------------------------------------------------------
  // --- LINKS ---

  public static getLinks(
    offset: number = 0,
    limit: number = 100
  ): Promise<LinkModel[]> {
    return this.get(`links?offset=${offset}&limit=${limit}`);
  }

  public static createLink(link: LinkCreateModel): Promise<LinkModel> {
    return this.post('links', link);
  }

  public static searchLinks(
    query: string,
    offset: number = 0,
    limit: number = 100
  ): Promise<LinkModel[]> {
    return this.get(
      `links/search?offset=${offset}&limit=${limit}&query=${query}`
    );
  }

  public static getLink(id: string): Promise<LinkModel> {
    return this.get(`links/${id}`);
  }

  public static updateLink(link: LinkCreateModel): Promise<LinkModel> {
    return this.post(`links/${link.guid}`, link);
  }

  public static deleteLink(id: string): Promise<any> {
    return this.delete(`links/${id}`);
  }

  // ------------------------------------------------------------
  // --- USERS ---

  public static getUsers(
    offset: number = 0,
    limit: number = 100
  ): Promise<UserModel[]> {
    return this.get(`users?offset=${offset}&limit=${limit}`);
  }

  public static createUser(link: UserCreateModel): Promise<UserModel> {
    return this.post('users', link);
  }

  public static getUser(id: string): Promise<UserModel> {
    return this.get(`users/${id}`);
  }

  public static updateUser(link: UserCreateModel): Promise<UserModel> {
    return this.post(`users/${link.guid}`, link);
  }

  public static deleteUser(id: string): Promise<any> {
    return this.delete(`users/${id}`);
  }

  public static getUserLinks(
    userId: string,
    offset: number = 0,
    limit: number = 100
  ): Promise<LinkModel[]> {
    return this.get(`users/${userId}/links?offset=${offset}&limit=${limit}`);
  }

  public static searchUserLinks(
    userId: string,
    query: string,
    offset: number = 0,
    limit: number = 100
  ): Promise<LinkModel[]> {
    return this.get(
      `users/${userId}/links/search?offset=${offset}&limit=${limit}&query=${query}`
    );
  }

  // ------------------------------------------------------------
  // --- HELPERS ---

  public static get<T>(path: string, emitError: boolean = true): Promise<T> {
    return this.req<T>('GET', path, undefined, undefined, emitError);
  }

  private static post<T>(
    path: string,
    body?: any,
    emitError: boolean = true
  ): Promise<T> {
    return this.req<T>('POST', path, body, undefined, emitError);
  }

  private static put<T>(
    path: string,
    body?: any,
    emitError: boolean = true
  ): Promise<T> {
    return this.req<T>('PUT', path, body, undefined, emitError);
  }

  private static delete<T>(
    path: string,
    emitError: boolean = true
  ): Promise<T> {
    return this.req<T>('DELETE', path, undefined, undefined, emitError);
  }

  private static async req<T>(
    method: string,
    path: string,
    body?: any,
    contentType: string | undefined = 'application/json',
    emitError: boolean = true
  ): Promise<T> {
    let reqBody = undefined;
    if (body) {
      if (typeof body !== 'string' && contentType === 'application/json') {
        reqBody = JSON.stringify(body);
      } else {
        reqBody = body;
      }
    }

    const headers: { [key: string]: string } = {};
    if (contentType !== 'multipart/form-data') {
      headers['content-type'] = contentType;
    }

    const res = await window.fetch(`${PREFIX}/${path}`, {
      method,
      headers,
      body: reqBody,
      credentials: 'include',
    });

    if (res.status === 401) {
      if (emitError) this.events.emit('authentication-error', res);
      throw new AuthenticationError();
    }

    if (!res.ok) {
      if (emitError) this.events.emit('error', res);
      throw new Error(res.statusText);
    }

    if (res.status === 204 || res.headers.get('content-length') === '0') {
      return {} as T;
    }

    return res.json();
  }
}
