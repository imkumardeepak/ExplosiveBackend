import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { PlantMaster } from '@/types/domain.types';

export const plantsService = {
  getAll: () => apiGet<PlantMaster[]>('/PlantMasters/GetAllPlants'),
  getById: (id: number) => apiGet<PlantMaster>(`/PlantMasters/GetPlantById/${id}`),
  getNames: () => apiGet<string[]>('/PlantMasters/GetPlantNames'),
  getCodeByName: (plantName: string) =>
    apiGet<string>(`/PlantMasters/GetPlantCodeByName/${plantName}`),
  checkCodeExists: (code: string) =>
    apiGet<boolean>(`/PlantMasters/CheckPlantCodeExists/${code}`),
  getByBrandName: (brandName: string) =>
    apiGet<string>(`/PlantMasters/GetPlantNameByBName/${brandName}`),
  create: (data: Omit<PlantMaster, 'id'>) => apiPost<PlantMaster>('/PlantMasters/CreatePlant', data),
  update: (id: number, data: PlantMaster) => apiPut<PlantMaster>(`/PlantMasters/UpdatePlant/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/PlantMasters/DeletePlant/${id}`),
};
