import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, ProfileFormData } from '../types';
import { authService } from '../services/authService';
import { getAuthToken, removeAuthToken, getErrorMessage } from '../utils';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  
  
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, name: string) => Promise<void>;
  logout: () => void;
  getCurrentUser: () => Promise<void>;
  updateUser: (data: ProfileFormData) => Promise<void>;
  clearError: () => void;
  setLoading: (loading: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      isLoading: false,
      error: null,

      login: async (email: string, password: string) => {
        set({ isLoading: true, error: null });
        try {
          await authService.login({ email, password });
          
          
          const user = await authService.getCurrentUser();
          
          set({
            user: user,
            isAuthenticated: true,
            isLoading: false,
            error: null,
          });
        } catch (error: unknown) {
          set({
            isLoading: false,
            error: getErrorMessage(error) || 'Login failed',
          });
          throw error;
        }
      },

      register: async (email: string, password: string, name: string) => {
        set({ isLoading: true, error: null });
        try {
          await authService.register({ email, password, name });
          set({ isLoading: false, error: null });
        } catch (error: unknown) {
          set({
            isLoading: false,
            error: getErrorMessage(error) || 'Registration failed',
          });
          throw error;
        }
      },

      logout: () => {
        authService.logout();
        set({
          user: null,
          isAuthenticated: false,
          error: null,
        });
      },

      getCurrentUser: async () => {
        const token = getAuthToken();
            if (!token) {
      set({ isAuthenticated: false, user: null });
      return;
    }

        set({ isLoading: true });
        try {
          const user = await authService.getCurrentUser();
          set({
            user: user,
            isAuthenticated: true,
            isLoading: false,
            error: null,
          });
        } catch (error: unknown) {
          if ((error as { response?: { status?: number } })?.response?.status === 401) {
            removeAuthToken();
            set({
              user: null,
              isAuthenticated: false,
              isLoading: false,
              error: null,
            });
          } else {
            set({
              isLoading: false,
              error: getErrorMessage(error) || 'Failed to get user profile',
            });
          }
        }
      },

      clearError: () => {
        set({ error: null });
      },

      setLoading: (loading: boolean) => {
        set({ isLoading: loading });
      },

      updateUser: async (data: ProfileFormData) => {
        set({ isLoading: true, error: null });
        try {
          const updatedUser = await authService.updateUser(data);
          set({
            user: updatedUser,
            isLoading: false,
            error: null,
          });
        } catch (error: unknown) {
          set({
            isLoading: false,
            error: getErrorMessage(error) || 'Failed to update profile',
          });
          throw error;
        }
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);
