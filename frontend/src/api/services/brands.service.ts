import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { BrandMaster } from '@/types/domain.types';

export const brandsService = {
  getAll: () => apiGet<BrandMaster[]>('/BrandMasters/GetAllBrands'),
  getById: (id: number) => apiGet<BrandMaster>(`/BrandMasters/GetBrandById/${id}`),
  getAllNames: () => apiGet<string[]>('/BrandMasters/GetAllBrandNames'),
  getIdByName: (brandName: string) =>
    apiGet<string>(`/BrandMasters/GetIdByBrandName/${brandName}`),
  checkExists: (bid: string) => apiGet<boolean>(`/BrandMasters/CheckBrandExists/${bid}`),
  getByPlantCode: (pcode: string) =>
    apiGet<string[]>(`/BrandMasters/GetBrandNamesByPCode/${pcode}`),
  getByPlantName: (pname: string) =>
    apiGet<string[]>(`/BrandMasters/GetBrandNamesByPName/${pname}`),
  create: (data: Omit<BrandMaster, 'id'>) => apiPost<BrandMaster>('/BrandMasters/CreateBrand', data),
  update: (id: number, data: BrandMaster) => apiPut<BrandMaster>(`/BrandMasters/UpdateBrand/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/BrandMasters/DeleteBrand/${id}`),
};
