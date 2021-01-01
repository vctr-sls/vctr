/** @format */

import { Permissions } from '../services/api/models';

export default class PermissionsUtil {
  public static readonly map: { [key: string]: Permissions } = (() => {
    let map: { [key: string]: Permissions } = {};

    let permNames = Object.keys(Permissions);
    permNames = permNames.slice(permNames.length / 2 + 1);

    let permValues = Object.values(Permissions);
    permValues = permValues.slice(permValues.length / 2 + 1);

    permNames.forEach((name, i) => (map[name] = permValues[i] as Permissions));

    return map;
  })();

  public static transformName(name: string) {
    return name
      .toLowerCase()
      .split('_')
      .map((n) => n[0].toUpperCase() + n.substr(1))
      .join(' ');
  }

  public static getNames(permissions: Permissions): string[] {
    let names: string[] = [];

    if (permissions === Permissions.ADMINISTRATOR) return ['Administrator'];

    const permNames = Object.keys(this.map);
    Object.values(this.map).forEach((pv, i) => {
      if (((pv as Permissions) & permissions) === pv) {
        const name = this.transformName(permNames[i]);
        names.push(name);
      }
    });

    return names;
  }
}
