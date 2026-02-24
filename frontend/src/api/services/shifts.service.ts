import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { ShiftMaster } from '@/types/domain.types';

export const shiftsService = {
  getAll: () => apiGet<ShiftMaster[]>('/ShiftMasters/GetAllShifts'),
  getById: (id: number) => apiGet<ShiftMaster>(`/ShiftMasters/GetShiftById/${id}`),
  create: (data: Omit<ShiftMaster, 'id'>) => apiPost<ShiftMaster>('/ShiftMasters/CreateShift', data),
  update: (id: number, data: ShiftMaster) => apiPut<ShiftMaster>(`/ShiftMasters/UpdateShift/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/ShiftMasters/DeleteShift/${id}`),
};
