/** @format */

type Timer = ReturnType<typeof setTimeout>;

export default class InputLimiter {
  private timer: Timer | null = null;

  constructor(private limitms: number) {}

  public static with(limitms: number) {
    return new InputLimiter(limitms);
  }

  public input<T>(val: T, cb: (val: T) => void) {
    if (this.timer) {
      clearTimeout(this.timer);
    }
    this.timer = setTimeout(() => cb(val), this.limitms);
  }
}
