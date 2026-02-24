import axiosInstance from './axiosInstance';
import type { APIResponse } from '@/types/api.types';

export const apiGet = async <T>(url: string, params?: Record<string, unknown>): Promise<T> => {
  const response = await axiosInstance.get<APIResponse<T>>(url, { params });
  return response.data.data;
};

export const apiPost = async <T>(url: string, data?: unknown): Promise<T> => {
  const response = await axiosInstance.post<APIResponse<T>>(url, data);
  return response.data.data;
};

export const apiPut = async <T>(url: string, data?: unknown): Promise<T> => {
  const response = await axiosInstance.put<APIResponse<T>>(url, data);
  return response.data.data;
};

export const apiPatch = async <T>(url: string, data?: unknown): Promise<T> => {
  const response = await axiosInstance.patch<APIResponse<T>>(url, data);
  return response.data.data;
};

export const apiDelete = async <T>(url: string): Promise<T> => {
  const response = await axiosInstance.delete<APIResponse<T>>(url);
  return response.data.data;
};

export const apiGetBlob = async (url: string, params?: Record<string, unknown>): Promise<Blob> => {
  const response = await axiosInstance.get(url, {
    params,
    responseType: 'blob',
  });
  return response.data;
};
