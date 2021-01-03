/** @format */

export default class ElementsUtil {
  public static repeat(
    n: number,
    elem: JSX.Element | ((i: number) => JSX.Element)
  ): JSX.Element[] {
    const res = [];
    for (let i = 0; i < n; i++)
      res.push(typeof elem === 'function' ? elem(i) : elem);
    return res;
  }
}
