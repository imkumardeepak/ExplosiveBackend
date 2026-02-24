import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { ProductMaster } from '@/types/domain.types';

export const productsService = {
  getAll: () => apiGet<ProductMaster[]>('/ProductMasters/GetAllProducts'),
  getById: (id: number) => apiGet<ProductMaster>(`/ProductMasters/GetProductById/${id}`),
  create: (data: Partial<ProductMaster>) =>
    apiPost<ProductMaster>('/ProductMasters/CreateProduct', data),
  update: (id: number, data: Partial<ProductMaster>) =>
    apiPut<ProductMaster>(`/ProductMasters/UpdateProduct/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/ProductMasters/DeleteProduct/${id}`),
};
