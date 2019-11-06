/** @format */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InitializeRouteComponent } from './routes/initialize/initialize.route';

const routes: Routes = [
  {
    path: 'initialize',
    component: InitializeRouteComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
