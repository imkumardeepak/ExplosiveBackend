import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { MfgMaster } from '@/types/domain.types';

export const mfgService = {
  getAll: () => apiGet<MfgMaster[]>('/MfgMasters/GetAllMfgMasters'),
  getById: (id: number) => apiGet<MfgMaster>(`/MfgMasters/GetMfgMasterById/${id}`),
  getByName: (name: string) => apiGet<MfgMaster>(`/MfgMasters/GetMfgMasterByName/${name}`),
  create: (data: Omit<MfgMaster, 'id'>) => apiPost<MfgMaster>('/MfgMasters/CreateMfgMaster', data),
  update: (id: number, data: MfgMaster) => apiPut<MfgMaster>(`/MfgMasters/UpdateMfgMaster/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/MfgMasters/DeleteMfgMaster/${id}`),
};
