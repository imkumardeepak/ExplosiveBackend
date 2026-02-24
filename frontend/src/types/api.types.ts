export interface APIResponse<T = unknown> {
  status: boolean;
  statusCode: number;
  data: T;
  message: string;
  errors: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  user: UserMaster;
  token: string;
}

export interface UserMaster {
  id: number;
  username: string;
  email: string;
  company_ID: string;
  role: RoleMaster;
  isActive: boolean;
}

export interface RoleMaster {
  id: number;
  roleName: string;
}

export type ApiError = {
  message: string;
  errors: string[];
  statusCode: number;
};
