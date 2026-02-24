import axios, { AxiosError } from 'axios';
import type { AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import env from '@/lib/env';
import { tokenStorage, isTokenExpired } from '@/utils/token';
import type { APIResponse } from '@/types/api.types';

const axiosInstance = axios.create({
  baseURL: env.API_BASE_URL,
  timeout: env.API_TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = tokenStorage.get();
    if (token && !isTokenExpired(token)) {
      config.headers.Authorization = `Bearer ${token}`;
    } else if (token && isTokenExpired(token)) {
      tokenStorage.remove();
      window.dispatchEvent(new CustomEvent('auth:session-expired'));
    }
    return config;
  },
  (error: AxiosError) => Promise.reject(error),
);

axiosInstance.interceptors.response.use(
  (response: AxiosResponse<APIResponse>) => response,
  (error: AxiosError<APIResponse>) => {
    if (error.response?.status === 401) {
      tokenStorage.remove();
      window.dispatchEvent(new CustomEvent('auth:unauthorized'));
    }

    if (error.response?.status === 403) {
      window.dispatchEvent(new CustomEvent('auth:forbidden'));
    }

    const apiError = {
      message:
        error.response?.data?.message ||
        error.message ||
        'An unexpected error occurred',
      errors: error.response?.data?.errors ?? [],
      statusCode: error.response?.status ?? 500,
    };

    return Promise.reject(apiError);
  },
);

export default axiosInstance;
