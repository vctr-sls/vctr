/** @format */

export enum Permissions {
  UNSET = -1,
  ADMINISTRATOR = 2147483647,
  VIEW_LINKS = 1 << 1,
  CREATE_LINKS = 1 << 2,
  UPDATE_LINKS = 1 << 3,
  DELETE_LINKS = 1 << 4,
  VIEW_USERS = 1 << 5,
  CREATE_USERS = 1 << 6,
  UPDATE_USERS = 1 << 7,
  DELETE_USERS = 1 << 8,
  PERFORM_STATE_CHANGES = 1 << 9,
}

export interface LoginModel {
  ident: string;
  password: string;
  remember: boolean;
}

export interface EntityModel {
  guid: string;
  created: Date;
}

export interface UserModel extends EntityModel {
  username: string;
  permissions: Permissions;
  last_login: Date;
}

export interface UserCreateModel extends UserModel {
  password: string;
}

export interface LinkModel extends EntityModel {
  ident: string;
  destination: string;
  creator: UserModel;
  enabled: boolean;
  permanent_redirect: boolean;
  password_required: boolean;
  last_access: Date;
  access_count: number;
  unique_access_count: number;
  total_access_limit: number;
  expires: Date;
}

export interface LinkCreateModel extends LinkModel {
  password: string;
}
