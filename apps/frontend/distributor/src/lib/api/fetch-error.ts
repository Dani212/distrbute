export class FetchError extends Error {
  constructor(
    message: string,
    public readonly status: number,
    public readonly statusText: string
  ) {
    super(message);
  }
}
