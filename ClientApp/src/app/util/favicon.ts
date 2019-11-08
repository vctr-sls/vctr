/** @format */

export function getFavicon(urlStr: string) {
  let url = new URL(urlStr);
  return url.origin + '/favicon.ico';
}
