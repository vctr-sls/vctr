/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';

@Component({
  selector: 'app-route-initialize',
  templateUrl: './initialize.route.html',
  styleUrls: ['./initialize.route.scss'],
})
export class InitializeRouteComponent {
  constructor(private api: APIService) {}
}
