import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';

export const useGlobalAuthEvents = () => {
  const logout = useAuthStore((s) => s.logout);
  const addToast = useUIStore((s) => s.addToast);
  const navigate = useNavigate();

  useEffect(() => {
    const handleUnauthorized = () => {
      logout();
      addToast({ type: 'error', message: 'Session expired. Please login again.' });
      navigate('/login', { replace: true });
    };

    const handleForbidden = () => {
      addToast({ type: 'error', message: 'You do not have permission for this action.' });
    };

    const handleSessionExpired = () => {
      logout();
      addToast({ type: 'warning', message: 'Your session has expired.' });
      navigate('/login', { replace: true });
    };

    window.addEventListener('auth:unauthorized', handleUnauthorized);
    window.addEventListener('auth:forbidden', handleForbidden);
    window.addEventListener('auth:session-expired', handleSessionExpired);

    return () => {
      window.removeEventListener('auth:unauthorized', handleUnauthorized);
      window.removeEventListener('auth:forbidden', handleForbidden);
      window.removeEventListener('auth:session-expired', handleSessionExpired);
    };
  }, [logout, addToast, navigate]);
};
