import { useEffect, useRef, useCallback } from 'react';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';
import env from '@/lib/env';

type WSMessage = {
  type: string;
  payload: unknown;
};

export const useWebSocket = (onMessage?: (msg: WSMessage) => void) => {
  const wsRef = useRef<WebSocket | null>(null);
  const token = useAuthStore((s) => s.token);
  const addToast = useUIStore((s) => s.addToast);

  const connect = useCallback(() => {
    if (!token || wsRef.current?.readyState === WebSocket.OPEN) return;

    const ws = new WebSocket(`${env.WS_URL}?token=${token}`);
    wsRef.current = ws;

    ws.onmessage = (event) => {
      try {
        const data: WSMessage = JSON.parse(event.data);
        onMessage?.(data);

        if (data.type === 'notification') {
          addToast({
            type: 'info',
            message: String(data.payload),
            duration: 5000,
          });
        }
      } catch {
        // non-JSON messages are ignored
      }
    };

    ws.onclose = () => {
      setTimeout(connect, 5000);
    };

    ws.onerror = () => {
      ws.close();
    };
  }, [token, onMessage, addToast]);

  useEffect(() => {
    connect();
    return () => {
      wsRef.current?.close();
    };
  }, [connect]);

  const send = useCallback((data: unknown) => {
    if (wsRef.current?.readyState === WebSocket.OPEN) {
      wsRef.current.send(JSON.stringify(data));
    }
  }, []);

  return { send };
};
