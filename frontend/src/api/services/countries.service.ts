import { apiGet, apiPost, apiPut, apiDelete } from '../baseService';
import type { CountryMaster } from '@/types/domain.types';

export const countriesService = {
  getAll: () => apiGet<CountryMaster[]>('/CountryMasters/GetAllCountries'),
  getById: (id: number) => apiGet<CountryMaster>(`/CountryMasters/GetCountryById/${id}`),
  create: (data: Omit<CountryMaster, 'id'>) => apiPost<CountryMaster>('/CountryMasters/CreateCountry', data),
  update: (id: number, data: CountryMaster) => apiPut<CountryMaster>(`/CountryMasters/UpdateCountry/${id}`, data),
  delete: (id: number) => apiDelete<void>(`/CountryMasters/DeleteCountry/${id}`),
};
