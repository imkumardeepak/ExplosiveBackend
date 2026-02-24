import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { CustomerMaster } from '@/types/domain.types';

export const customersService = {
  getAll: () => apiGet<CustomerMaster[]>('/CustomerMasters/GetAllCustomers'),
  getById: (id: number) => apiGet<CustomerMaster>(`/CustomerMasters/GetCustomerById/${id}`),
  getNames: () => apiGet<string[]>('/CustomerMasters/GetcustomerNames'),
  getIdByName: (custName: string) =>
    apiGet<number>(`/CustomerMasters/GetCustomersid/${custName}`),
  create: (data: Omit<CustomerMaster, 'id'>) =>
    apiPost<CustomerMaster>('/CustomerMasters/CreateCustomer', data),
  update: (id: number, data: CustomerMaster) =>
    apiPut<CustomerMaster>(`/CustomerMasters/UpdateCustomer/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/CustomerMasters/DeleteCustomer/${id}`),
};
