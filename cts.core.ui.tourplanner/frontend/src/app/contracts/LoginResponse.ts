export interface LoginResponse {
  userGuid: string;
  email: string;
  createdAt: Date;
  accessToken: string;
  accessTokenExpiresAtUtc: Date;
}
