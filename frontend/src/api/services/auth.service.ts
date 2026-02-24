import { apiGet, apiPost } from '../baseService';
import type { LoginRequest, LoginResponse } from '@/types/api.types';

export const authService = {
  login: (data: LoginRequest) => apiPost<LoginResponse>('/Login/Login', data),
};
