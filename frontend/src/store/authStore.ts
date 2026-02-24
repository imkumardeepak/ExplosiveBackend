import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { AuthState, AuthUser } from '@/types/auth.types';
import { tokenStorage, parseJwt } from '@/utils/token';
import env from '@/lib/env';

interface AuthActions {
  setToken: (token: string) => void;
  setUser: (user: AuthUser) => void;
  logout: () => void;
  hydrateFromToken: () => void;
}

type AuthStore = AuthState & AuthActions;

export const useAuthStore = create<AuthStore>()(
  persist(
    (set) => ({
      token: null,
      user: null,
      isAuthenticated: false,
      isLoading: false,

      setToken: (token: string) => {
        tokenStorage.set(token);
        const decoded = parseJwt(token);
        const user: AuthUser | null = decoded
          ? {
              id: Number(decoded['Id']),
              username: decoded[
                'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
              ] as string,
              email: '',
              companyId: decoded['CompanyId'] as string,
              role: decoded[
                'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
              ] as string,
            }
          : null;

        set({ token, user, isAuthenticated: true });
      },

      setUser: (user: AuthUser) => set({ user }),

      logout: () => {
        tokenStorage.remove();
        set({ token: null, user: null, isAuthenticated: false });
      },

      hydrateFromToken: () => {
        const token = tokenStorage.get();
        if (!token) return;
        const decoded = parseJwt(token);
        if (!decoded) return;
        const user: AuthUser = {
          id: Number(decoded['Id']),
          username: decoded[
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
          ] as string,
          email: '',
          companyId: decoded['CompanyId'] as string,
          role: decoded[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
          ] as string,
        };
        set({ token, user, isAuthenticated: true });
      },
    }),
    {
      name: env.TOKEN_KEY,
      partialize: (state) => ({ token: state.token }),
    },
  ),
);
