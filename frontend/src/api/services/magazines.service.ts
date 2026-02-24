import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { MagazineMaster } from '@/types/domain.types';

export const magazinesService = {
  getAll: () => apiGet<MagazineMaster[]>('/MagzineMasters/GetAllMagzines'),
  getById: (id: number) => apiGet<MagazineMaster>(`/MagzineMasters/GetMagzineById/${id}`),
  getNames: () => apiGet<string[]>('/MagzineMasters/GetmagNames'),
  getByCode: (mcode: string) => apiGet<MagazineMaster>(`/MagzineMasters/GetMagzineByMCode/${mcode}`),
  checkExists: (mcode: string) => apiGet<boolean>(`/MagzineMasters/CheckMagazineExists/${mcode}`),
  getLicByCode: (mcode: string) => apiGet<string>(`/MagzineMasters/GetLicByMCode/${mcode}`),
  create: (data: Omit<MagazineMaster, 'id'>) =>
    apiPost<MagazineMaster>('/MagzineMasters/CreateMagzine', data),
  update: (id: number, data: MagazineMaster) =>
    apiPut<MagazineMaster>(`/MagzineMasters/UpdateMagzine/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/MagzineMasters/DeleteMagzine/${id}`),
};
