import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { TransportMaster } from '@/types/domain.types';

export const transportService = {
  getAll: () => apiGet<TransportMaster[]>('/TransportMasters/GetAllTransportMasters'),
  getById: (id: number) =>
    apiGet<TransportMaster>(`/TransportMasters/GetTransportMasterById/${id}`),
  getByName: (name: string) =>
    apiGet<TransportMaster>(`/TransportMasters/GetTransportByName/${name}`),
  create: (data: Omit<TransportMaster, 'id'>) =>
    apiPost<TransportMaster>('/TransportMasters/CreateTransportMaster', data),
  update: (id: number, data: TransportMaster) =>
    apiPut<TransportMaster>(`/TransportMasters/UpdateTransportMaster/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/TransportMasters/DeleteTransportMaster/${id}`),
};
