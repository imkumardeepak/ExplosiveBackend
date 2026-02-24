import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { UserMaster, RoleMaster } from '@/types/domain.types';

export const usersService = {
  getAll: () => apiGet<UserMaster[]>('/Users/GetAllUsers'),
  getById: (id: number) => apiGet<UserMaster>(`/Users/GetUserById/${id}`),
  getByName: (username: string) => apiGet<UserMaster>(`/Users/GetUserByName/${username}`),
  create: (data: Partial<UserMaster> & { password: string }) =>
    apiPost<UserMaster>('/Users/PostUser', data),
  update: (id: number, data: Partial<UserMaster> & { password?: string }) =>
    apiPut<UserMaster>(`/Users/PutUser/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/Users/DeleteUser/${id}`),
};

export const rolesService = {
  getAll: () => apiGet<RoleMaster[]>('/RoleMaster/GetRoleMasterList'),
  getById: (id: number) => apiGet<RoleMaster>(`/RoleMaster/GetRoleMasterById/${id}`),
  create: (data: Omit<RoleMaster, 'id'>) =>
    apiPost<RoleMaster>('/RoleMaster/CreateRoleMaster', data),
  update: (id: number, data: RoleMaster) =>
    apiPut<RoleMaster>(`/RoleMaster/UpdateRoleMaster/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/RoleMaster/DeleteRoleMaster/${id}`),
};
