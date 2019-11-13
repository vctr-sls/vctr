/** @format */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';
import { MainRouteComponent } from './routes/main/main.route';
import { LoginRouteComponent } from './routes/login/login.route';
import { EditRouteComponent } from './routes/edit/edit.route';
import { ErrorRouteComponent } from './routes/error/error.route';
import { ProtectedRouteComponent } from './routes/protected/protected.route';
import { SettingsRouteComponent } from './routes/settings/settings.route';

const routes: Routes = [
  {
    path: '',
    component: MainRouteComponent,
  },
  {
    path: 'initialize',
    component: InitializeRouteComponent,
  },
  {
    path: 'login',
    component: LoginRouteComponent,
  },
  {
    path: 'protected',
    component: ProtectedRouteComponent,
  },
  {
    path: 'settings',
    component: SettingsRouteComponent,
  },
  {
    path: 'error/:type',
    component: ErrorRouteComponent,
  },
  {
    path: ':guid/edit',
    component: EditRouteComponent,
  },
  {
    path: '**',
    pathMatch: 'full',
    redirectTo: '/',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
