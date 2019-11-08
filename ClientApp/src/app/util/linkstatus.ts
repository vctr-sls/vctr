/** @format */

import { ShortLink } from '../api/api.models';

export interface LinkStatus {
  active: boolean;
  text: string;
  type: number;
}

export function getLinkStatus(shortLink: ShortLink): LinkStatus {
  let status = {
    active: false,
    text: 'This short link is publicly available.',
  } as LinkStatus;

  if (!shortLink.isActive) {
    status.text = 'The link is unavailable because it is actively deactivated.';
    status.type = 1;
  } else if (
    shortLink.maxUses > 0 &&
    shortLink.uniqueAccessCount >= shortLink.maxUses
  ) {
    status.text =
      'The link is unavailable because max access count was exceed.';
    status.type = 2;
  } else if (shortLink.activates > new Date()) {
    status.text =
      'The link is unavailable because activation date is not reached yet.';
    status.type = 3;
  } else if (shortLink.expires < new Date()) {
    status.text = 'The link is unavailable because expire date was reached.';
    status.type = 4;
  } else {
    status.active = true;
    status.type = 0;
  }

  return status;
}
