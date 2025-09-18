import { create } from 'zustand';
import notificationService, { 
  Notification, 
  NotificationPreferences,
  GetNotificationsRequest
} from '../services/notificationService';
import { toastService } from '../services/toastService';
import { getErrorMessage } from '../utils';

interface NotificationStore {
  notifications: Notification[];
  unreadCount: number;
  pageSize: number;
  page: number;
  totalPages: number;
  totalCount: number;
  isLoading: boolean;
  error: string | null;
  preferences: NotificationPreferences | null;
  isPreferencesLoading: boolean;
  preferencesError: string | null;
  
  
  fetchNotifications: (params?: GetNotificationsRequest) => Promise<void>;
  fetchUnreadCount: () => Promise<void>;
  markAsRead: (id: string) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  deleteNotification: (id: string) => Promise<void>;
  fetchPreferences: () => Promise<void>;
  updatePreferences: (preferences: Partial<NotificationPreferences>) => Promise<void>;

}

export const useNotificationStore = create<NotificationStore>((set) => ({
  notifications: [],
  unreadCount: 0,
  pageSize: 20,
  page: 1,
  totalPages: 1,
  totalCount: 0,
  isLoading: false,
  error: null,
  preferences: null,
  isPreferencesLoading: false,
  preferencesError: null,

  fetchNotifications: async (params?: GetNotificationsRequest) => {
    try {
      set({ isLoading: true, error: null });
      const response = await notificationService.getNotifications(params);
      set({
        notifications: response.notifications,
        totalCount: response.totalCount,
        page: response.page,
        pageSize: response.pageSize,
        totalPages: response.totalPages
      });
    } catch (error) {
      set({ error: error instanceof Error ? getErrorMessage(error) : 'Failed to fetch notifications' });
    } finally {
      set({ isLoading: false });
    }
  },

  fetchUnreadCount: async () => {
    try {
      const response = await notificationService.getUnreadCount();
      if (response.success) {
        set({ unreadCount: response.count });
      }
    } catch (error) {
      console.error('Failed to fetch unread count:', error);
    }
  },

  markAsRead: async (id: string) => {
    try {
      const response = await notificationService.markAsRead(id);
      if (response.success) {
        set(state => ({
          notifications: state.notifications.map(n => 
            n.id === id ? { ...n, isRead: true, isUnread: false } : n
          ),
          unreadCount: Math.max(0, state.unreadCount - 1)
        }));
      }
    } catch {
      toastService.error('Error', 'Failed to mark notification as read');
    }
  },

  markAllAsRead: async () => {
    try {
      const response = await notificationService.markAllAsRead();
      if (response.success) {
        set(state => ({
          notifications: state.notifications.map(n => ({ ...n, isRead: true, isUnread: false })),
          unreadCount: 0
        }));
      }
    } catch {
      toastService.error('Error', 'Failed to mark all notifications as read');
    }
  },

  deleteNotification: async (id: string) => {
    try {
      const response = await notificationService.deleteNotification(id);
      if (response.success) {
        set(state => {
          const notification = state.notifications.find(n => n.id === id);
          return {
            notifications: state.notifications.filter(n => n.id !== id),
            unreadCount: notification?.isUnread ? Math.max(0, state.unreadCount - 1) : state.unreadCount
          };
        });
      }
    } catch {
      toastService.error('Error', 'Failed to delete notification');
    }
  },

  fetchPreferences: async () => {
    try {
      set({ isPreferencesLoading: true, preferencesError: null });
      const response = await notificationService.getPreferences();
      if (response.success) {
        set({ preferences: response.preferences });
      }
    } catch (error) {
      set({ preferencesError: error instanceof Error ? getErrorMessage(error) : 'Failed to fetch preferences' });
    } finally {
      set({ isPreferencesLoading: false });
    }
  },

  updatePreferences: async (preferences: Partial<NotificationPreferences>) => {
    try {
      const response = await notificationService.updatePreferences(preferences);
      if (response.success) {
        set({ preferences: response.preferences });
        toastService.success('Success', 'Notification preferences updated');
      }
    } catch {
      toastService.error('Error', 'Failed to update notification preferences');
    }
  },


}));