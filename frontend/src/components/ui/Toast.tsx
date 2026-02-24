import { useEffect } from 'react';
import { useUIStore } from '@/store/uiStore';

export const ToastContainer = () => {
  const toasts = useUIStore((s) => s.toasts);
  const removeToast = useUIStore((s) => s.removeToast);

  return (
    <div className="toast-container">
      {toasts.map((toast) => (
        <ToastItem
          key={toast.id}
          id={toast.id}
          type={toast.type}
          message={toast.message}
          duration={toast.duration ?? 4000}
          onRemove={removeToast}
        />
      ))}
    </div>
  );
};

interface ToastItemProps {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
  duration: number;
  onRemove: (id: string) => void;
}

const ToastItem = ({ id, type, message, duration, onRemove }: ToastItemProps) => {
  useEffect(() => {
    const timer = setTimeout(() => onRemove(id), duration);
    return () => clearTimeout(timer);
  }, [id, duration, onRemove]);

  return (
    <div className={`toast toast--${type}`} role="alert">
      <span className="toast__message">{message}</span>
      <button
        className="toast__close"
        onClick={() => onRemove(id)}
        aria-label="Close"
      >
        ×
      </button>
    </div>
  );
};
