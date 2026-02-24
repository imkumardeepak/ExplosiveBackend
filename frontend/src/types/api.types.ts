export interface APIResponse<T = unknown> {
  status: boolean;
  statusCode: number;
  data: T;
  message: string;
  errors: string[];
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  user: UserEntity;
  token: string;
}

export interface UserEntity {
  id: number;
  username: string;
  email: string;
  company_ID: string;
  fullname?: string;
  role: RoleEntity;
  isActive?: boolean;
}

export interface RoleEntity {
  id: number;
  roleName: string;
  pageAccesses?: PageAccess[];
}

export interface PageAccess {
  pageName: string;
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}

export type ApiError = {
  message: string;
  errors: string[];
  statusCode: number;
};
