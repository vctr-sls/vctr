/** @format */

export default class UrlUtils {
  public static formatDestination(url: string): string {
    return url.replace(/(?:https?:\/\/)?(?:www\.)?/g, '');
  }
}
