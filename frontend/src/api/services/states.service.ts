import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { StateMaster } from '@/types/domain.types';

export const statesService = {
  getAll: () => apiGet<StateMaster[]>('/StateMasters/GetAllStates'),
  getById: (id: number) => apiGet<StateMaster>(`/StateMasters/GetStateById/${id}`),
  create: (data: Omit<StateMaster, 'id'>) => apiPost<StateMaster>('/StateMasters/CreateState', data),
  update: (id: number, data: StateMaster) => apiPut<StateMaster>(`/StateMasters/UpdateState/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/StateMasters/DeleteState/${id}`),
};
