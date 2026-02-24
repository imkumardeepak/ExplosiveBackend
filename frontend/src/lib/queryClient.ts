import { QueryClient } from '@tanstack/react-query';
import type { ApiError } from '@/types/api.types';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5,
      gcTime: 1000 * 60 * 10,
      retry: (failureCount, error) => {
        const apiError = error as unknown as ApiError;
        if (apiError?.statusCode === 401 || apiError?.statusCode === 403) {
          return false;
        }
        return failureCount < 2;
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: false,
    },
  },
});
