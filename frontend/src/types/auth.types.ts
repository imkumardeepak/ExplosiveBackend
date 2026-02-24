export interface AuthState {
  token: string | null;
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

export interface AuthUser {
  id: number;
  username: string;
  email: string;
  companyId: string;
  role: string;
}

export interface DecodedToken {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
  Id: string;
  CompanyId: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  nbf: number;
  exp: number;
  iat: number;
}
