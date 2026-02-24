import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { authService } from '@/api';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';
import type { LoginRequest } from '@/types/api.types';
import type { ApiError } from '@/types/api.types';

export const useLogin = () => {
  const setToken = useAuthStore((s) => s.setToken);
  const addToast = useUIStore((s) => s.addToast);
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (credentials: LoginRequest) => authService.login(credentials),
    onSuccess: (data) => {
      setToken(data.token);
      addToast({ type: 'success', message: 'Login successful', duration: 3000 });
      navigate('/dashboard', { replace: true });
    },
    onError: (error: ApiError) => {
      addToast({
        type: 'error',
        message: error.message || 'Invalid credentials',
        duration: 5000,
      });
    },
  });
};

export const useLogout = () => {
  const logout = useAuthStore((s) => s.logout);
  const navigate = useNavigate();

  return () => {
    logout();
    navigate('/login', { replace: true });
  };
};

export const useCurrentUser = () => useAuthStore((s) => s.user);
export const useIsAuthenticated = () => useAuthStore((s) => s.isAuthenticated);
export const useUserRole = () => useAuthStore((s) => s.user?.role ?? null);
