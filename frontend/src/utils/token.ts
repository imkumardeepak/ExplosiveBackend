import env from '@/lib/env';

export const tokenStorage = {
  get: (): string | null => localStorage.getItem(env.TOKEN_KEY),
  set: (token: string): void => localStorage.setItem(env.TOKEN_KEY, token),
  remove: (): void => localStorage.removeItem(env.TOKEN_KEY),
};

export const parseJwt = (token: string): Record<string, unknown> | null => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(window.atob(base64));
  } catch {
    return null;
  }
};

export const isTokenExpired = (token: string): boolean => {
  const decoded = parseJwt(token);
  if (!decoded || typeof decoded.exp !== 'number') return true;
  return decoded.exp * 1000 < Date.now();
};
