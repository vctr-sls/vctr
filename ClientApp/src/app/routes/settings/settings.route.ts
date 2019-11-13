/** @format */

import { Component, OnInit } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { GeneralSettingsPost, GeneralSettings } from 'src/app/api/api.models';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-route-settings',
  templateUrl: './settings.route.html',
  styleUrls: ['./settings.route.scss'],
})
export class SettingsRouteComponent implements OnInit {
  public settings: GeneralSettings;

  public password = {
    current: '',
    new: '',
    newRepeat: '',
  };

  constructor(private api: APIService, private snackBar: MatSnackBar) {}

  public ngOnInit() {
    this.api.settingsGet().subscribe((settings) => {
      this.settings = settings;
    });
  }

  public get passwordValid(): boolean {
    return (
      this.password.current &&
      this.password.new &&
      this.password.new === this.password.newRepeat
    );
  }

  public onPasswordChange() {
    const settings = {
      currentPassword: this.password.current,
      password: this.password.new,
    } as GeneralSettingsPost;

    this.api.settingsSet(settings).then(() => {
      this.snackBar.open('New Master Password set.', 'Ok');
    });
  }

  public onRedirectChange() {
    const settings = {
      defaultRedirect: this.settings.defaultRedirect || '__RESET__',
    } as GeneralSettingsPost;

    this.api.settingsSet(settings).then(() => {
      this.snackBar.open(
        `Default Redirect ${this.settings.defaultRedirect ? '' : 're'}set.`,
        'Ok'
      );
    });
  }
}
