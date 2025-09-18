import { toast } from 'react-toastify';

interface ToastOptions {
  position?: 'top-right' | 'top-center' | 'top-left' | 'bottom-right' | 'bottom-center' | 'bottom-left';
  autoClose?: number;
  hideProgressBar?: boolean;
  closeOnClick?: boolean;
  pauseOnHover?: boolean;
  draggable?: boolean;
  progress?: number;
}

const defaultOptions: ToastOptions = {
  position: 'top-right',
  autoClose: 3000,
  hideProgressBar: false,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
};

export const toastService = {
  show: (message: string, type: 'info' | 'success' | 'warning' | 'error' = 'info', options?: ToastOptions) => {
    toast(message, {
      ...defaultOptions,
      ...options,
      type,
    });
  },

  info: (title: string, message?: string, duration?: number) => {
    toastService.show(message || title, 'info', { autoClose: duration });
  },

  success: (title: string, message?: string, duration?: number) => {
    toastService.show(message || title, 'success', { autoClose: duration });
  },

  warning: (title: string, message?: string, duration?: number) => {
    toastService.show(message || title, 'warning', { autoClose: duration });
  },

  error: (title: string, message?: string, duration?: number) => {
    toastService.show(message || title, 'error', { autoClose: duration });
  },
};

export default toastService;