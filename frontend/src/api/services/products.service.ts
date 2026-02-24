import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { ProductMaster } from '@/types/domain.types';

export const productsService = {
  getAll: () => apiGet<ProductMaster[]>('/ProductMasters/GetAllProducts'),
  getById: (id: number) => apiGet<ProductMaster>(`/ProductMasters/GetProductById/${id}`),
  checkExists: (bid: string, psizecode: string) =>
    apiGet<boolean>(`/ProductMasters/CheckProductExists/${bid}/${psizecode}`),
  getSizeNames: (bid: string) => apiGet<string[]>(`/ProductMasters/GetPsizeNames/${bid}`),
  getProductDetails: (bid: string, psizecode: string) =>
    apiGet<ProductMaster>(`/ProductMasters/GetProductDetails/${bid}/${psizecode}`),
  create: (data: Omit<ProductMaster, 'id'>) =>
    apiPost<ProductMaster>('/ProductMasters/CreateProduct', data),
  update: (id: number, data: ProductMaster) =>
    apiPut<ProductMaster>(`/ProductMasters/UpdateProduct/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/ProductMasters/DeleteProduct/${id}`),
};
