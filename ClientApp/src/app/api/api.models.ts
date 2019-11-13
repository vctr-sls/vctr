/** @format */

export interface GeneralSettings {
  defaultRedirect: string;
}

export interface GeneralSettingsPost {
  currentPassword: string;
  password: string;
  defaultRedirect: string;
}

export interface SetPasswordPost {
  password: string;
  reset: boolean;
}

export interface ShortLink {
  guid: string;
  rootURL: string;
  shortIdent: string;
  maxUses: number;
  isActive: boolean;
  activates: Date;
  expires: Date;
  isPermanentRedirect: boolean;

  isPasswordProtected: boolean;
  accessCount: number;
  uniqueAccessCount: number;
  creationDate: Date;
  lastAccess: Date;
  lastModified: Date;
}

export interface ProtectedLogin {
  password: string;
  disableTracking: boolean;
}

export interface ProtectedResponse {
  rootURL: string;
}
