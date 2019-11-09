/** @format */

import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

const ERROR_TYPES = {
  invalid: {
    title: 'This short link does not exist or is no more accessable. :(',
    message:
      'The specified short link either does not exist or is no more accessable due to the configuration by the host of this service.',
  } as ErrorType,
};

interface ErrorType {
  title: string;
  message: string;
}

@Component({
  selector: 'app-route-error',
  templateUrl: './error.route.html',
  styleUrls: ['./error.route.scss'],
})
export class ErrorRouteComponent {
  public errorType: ErrorType;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.route.params.subscribe((params) => {
      this.errorType = ERROR_TYPES[params.type];
      if (!this.errorType) {
        this.router.navigate(['/']);
      }
    });
  }
}
