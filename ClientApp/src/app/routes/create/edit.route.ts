/** @format */

import { Component } from '@angular/core';
import { APIService } from 'src/app/api/api.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ShortLink } from 'src/app/api/api.models';
import dateFormat from 'dateformat';
import { randomIdent } from 'src/app/util/random';

export interface LinkStatus {
  active: boolean;
  text: string;
}

@Component({
  selector: 'app-route-edit',
  templateUrl: './edit.route.html',
  styleUrls: ['./edit.route.scss'],
})
export class EditRouteComponent {
  public isNew = false;
  public shortLink = {
    shortIdent: randomIdent(),
    isActive: true,
    expires: new Date('9999-12-31T23:59:59.00'),
  } as ShortLink;

  constructor(
    private api: APIService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.route.params.subscribe((params) => {
      if (params.guid === 'new') {
        this.isNew = true;
      } else {
        this.api.slGetSingle(params.guid).subscribe((shortLink) => {
          this.shortLink = shortLink;
          console.log(this.shortLink.shortIdent);
        });
      }
    });
  }

  public getIsoFormattedDate(date: Date): string {
    return dateFormat(date, "yyyy-mm-dd'T'HH:MM:ss.00");
  }

  public get linkStatus(): LinkStatus {
    let status = {
      active: false,
      text: 'This short link is publicly available.',
    } as LinkStatus;

    if (!this.shortLink.isActive) {
      status.text =
        'The link is unavailable because it is actively deactivated.';
    } else if (
      this.shortLink.maxUses > 0 &&
      this.shortLink.uniqueAccessCount >= this.shortLink.maxUses
    ) {
      status.text =
        'The link is unavailable because max access count was exceed.';
    } else if (this.shortLink.activates > new Date()) {
      status.text =
        'The link is unavailable because activation date is not reached yet.';
    } else if (this.shortLink.expires < new Date()) {
      status.text = 'The link is unavailable because expire date was reached.';
    } else {
      status.active = true;
    }

    return status;
  }

  public get today(): string {
    return this.getIsoFormattedDate(new Date());
  }

  public get isValid(): boolean {
    return (
      !!this.shortLink &&
      !!this.shortLink.rootURL &&
      !!this.shortLink.shortIdent
    );
  }

  public onActivationSet(event: any) {
    this.shortLink.activates = new Date(event.target.value);
  }

  public onExpiringSet(event: any) {
    this.shortLink.expires = new Date(event.target.value);
  }

  public action() {
    if (this.isNew) {
      this.api
        .slCreate(this.shortLink)
        .then(() => {
          this.router.navigate(['/']);
        })
        .catch((err) => {
          console.error(err);
        });
    } else {
      this.api
        .slEdit(this.shortLink)
        .then(() => {
          this.router.navigate(['/']);
        })
        .catch((err) => {
          console.error(err);
        });
    }
  }
}
