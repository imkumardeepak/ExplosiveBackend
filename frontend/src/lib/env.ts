const env = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL as string,
  WS_URL: import.meta.env.VITE_WS_URL as string,
  APP_NAME: import.meta.env.VITE_APP_NAME as string,
  TOKEN_KEY: import.meta.env.VITE_TOKEN_KEY as string,
  REFRESH_TOKEN_KEY: import.meta.env.VITE_REFRESH_TOKEN_KEY as string,
  API_TIMEOUT: Number(import.meta.env.VITE_API_TIMEOUT) || 30000,
  IS_DEV: import.meta.env.DEV,
  IS_PROD: import.meta.env.PROD,
} as const;

export default env;
