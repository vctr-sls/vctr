/** @format */

import { ShortLink } from '../api/api.models';

export function getShortLinkURL(sl: ShortLink): string {
  return `${window.location.origin}/${sl.shortIdent}`;
}
