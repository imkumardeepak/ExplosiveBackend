import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { MfgLocationMaster } from '@/types/domain.types';

export const mfgLocationsService = {
  getAll: () => apiGet<MfgLocationMaster[]>('/MfgLocationMasters/GetAllLocations'),
  getById: (id: number) => apiGet<MfgLocationMaster>(`/MfgLocationMasters/GetLocationById/${id}`),
  getNames: () => apiGet<string[]>('/MfgLocationMasters/GetMfgLocations'),
  getCodeByName: (name: string) => apiGet<string>(`/MfgLocationMasters/GetCodeByLocationName/${name}`),
  create: (data: Omit<MfgLocationMaster, 'id'>) =>
    apiPost<MfgLocationMaster>('/MfgLocationMasters/CreateLocation', data),
  update: (id: number, data: MfgLocationMaster) =>
    apiPut<MfgLocationMaster>(`/MfgLocationMasters/UpdateLocation/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/MfgLocationMasters/DeleteLocation/${id}`),
};
