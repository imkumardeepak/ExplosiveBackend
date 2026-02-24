import { apiGet, apiPost, apiDelete } from '../baseService';
import type { ProductionPlan } from '@/types/domain.types';

export const productionService = {
  getAllPlans: () => apiGet<ProductionPlan[]>('/ProductionPlan/GetAllPlans'),
  getPlanById: (id: number) => apiGet<ProductionPlan>(`/L1Generate/GetplanById/${id}`),
  createPlan: (data: Omit<ProductionPlan, 'id'>) =>
    apiPost<ProductionPlan>('/ProductionPlan/CreatePlan', data),
  updatePlan: (id: number, data: ProductionPlan) =>
    apiPost<ProductionPlan>(`/ProductionPlan/UpdatePlan/${id}`, data),
  deletePlan: (id: number) => apiDelete<void>(`/ProductionPlan/DeletePlan/${id}`),
  getPlanData: () => apiGet<ProductionPlan[]>('/L1Generate/GetdataPlan'),
};
