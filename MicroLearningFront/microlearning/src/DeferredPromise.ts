
export default interface DeferredPromise<T> extends Promise<T> {
  resolve (result: T): void;
  reject (readon?: any): void;
}

class DeferredPromiseImpl<T> {
  private _promise!: Promise<T>
  private _resolve!: (result: T) => void
  private _reject!: (reason?: any) => void
  public then!: any
  public catch!: any

  constructor () {
    this._promise = new Promise<T>((resolve, reject) => {
      this._resolve = resolve
      this._reject = reject
    })

    // bind `then` and `catch` to implement the same interface as Promise
    this.then = this._promise.then.bind(this._promise)
    this.catch = this._promise.catch.bind(this._promise);
    (this as any[any])[Symbol.toStringTag] = 'Promise'
  }

  public resolve (result: T): void {
    this._resolve(result)
  }

  public reject (reason?: any): void {
    this._reject(reason)
  }

  // then<TResult1 = T, TResult2 = never>(onfulfilled?: ((value: T) => TResult1 | PromiseLike<TResult1>) | undefined | null, onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | undefined | null): Promise<TResult1 | TResult2>;
  // catch<TResult = never>(onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | undefined | null): Promise<T | TResult>;
}

export function deferred<T> (): DeferredPromise<T> {
  return (new DeferredPromiseImpl<T>() as unknown) as DeferredPromise<T>
}
