/** @format */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';
import { MainRouteComponent } from './routes/main/main.route';
import { LoginRouteComponent } from './routes/login/login.route';
import { EditRouteComponent } from './routes/create/edit.route';

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
