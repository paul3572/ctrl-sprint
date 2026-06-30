import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthTokenStore {
  private token: string | null = null;

  set(token: string) {
    this.token = token;
  }

  get() {
    return this.token;
  }

  clear() {
    this.token = null;
  }
}
