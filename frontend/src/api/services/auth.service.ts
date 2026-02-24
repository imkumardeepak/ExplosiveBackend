import axiosInstance from '../axiosInstance';
import type { APIResponse, LoginRequest, LoginResponse } from '@/types/api.types';

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post<APIResponse<LoginResponse>>(
      '/Login/Login',
      credentials,
    );
    return response.data.data;
  },
};
