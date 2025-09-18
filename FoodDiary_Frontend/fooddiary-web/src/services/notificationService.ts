import { getAuthToken } from '../utils';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

export enum NotificationType {
  WaterReminder = 'WaterReminder',
  MealReminder = 'MealReminder',
  CalorieLimitWarning = 'CalorieLimitWarning',
  GoalAchievement = 'GoalAchievement',
  WeeklyProgress = 'WeeklyProgress',
  DailySummary = 'DailySummary'
}

export enum NotificationPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Urgent = 'Urgent'
}

export enum NotificationStatus {
  Unread = 'Unread',
  Read = 'Read'
}

export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
  priority: NotificationPriority;
  status: NotificationStatus;
  contextData: string;
  imageUrl?: string;
  createdAt: string;
  readAt?: string;
  isUnread: boolean;
  isRead: boolean;
  age: string;
  isRecent: boolean;
  priorityColor: string;
  typeIcon: string;
}

export interface NotificationPreferences {
  waterReminders: boolean;
  mealReminders: boolean;
  calorieWarnings: boolean;
  goalAchievements: boolean;
  weeklyProgress: boolean;
  dailySummary: boolean;
  waterTime: string;
  breakfastTime: string;
  lunchTime: string;
  dinnerTime: string;
  waterFrequency: number;
  weekendNotifications: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface GetNotificationsRequest {
  page?: number;
  pageSize?: number;
  includeRead?: boolean;
}

export interface GetNotificationsResponse {
  notifications: Notification[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  message: string;
}

export interface GetUnreadCountResponse {
  success: boolean;
  count: number;
  message: string;
}

export interface MarkAsReadResponse {
  success: boolean;
  message: string;
}

export interface MarkAllAsReadResponse {
  success: boolean;
  message: string;
  updatedCount: number;
}

export interface DeleteNotificationResponse {
  success: boolean;
  message: string;
}

export interface GetNotificationPreferencesResponse {
  success: boolean;
  message: string;
  preferences: NotificationPreferences;
}

export interface UpdateNotificationPreferencesRequest {
  waterReminders?: boolean;
  mealReminders?: boolean;
  calorieWarnings?: boolean;
  goalAchievements?: boolean;
  weeklyProgress?: boolean;
  dailySummary?: boolean;
  waterTime?: string;
  breakfastTime?: string;
  lunchTime?: string;
  dinnerTime?: string;
  waterFrequency?: number;
  weekendNotifications?: boolean;
}

export interface UpdateNotificationPreferencesResponse {
  success: boolean;
  message: string;
  preferences: NotificationPreferences;
}

export interface TestNotificationResponse {
  success: boolean;
  message: string;
  timestamp: string;
}

export interface CleanupNotificationsResponse {
  success: boolean;
  message: string;
  deletedCount: number;
}

const notificationService = {
  async getNotifications(params?: GetNotificationsRequest): Promise<GetNotificationsResponse> {
    const token = getAuthToken();
    if (!token) {
      return {
        notifications: [],
        totalCount: 0,
        page: 1,
        pageSize: 20,
        totalPages: 0,
        message: "No authentication token found"
      };
    }

    try {
      
      const page = params?.page ?? 1;
      const pageSize = params?.pageSize ?? 20;
      const includeRead = params?.includeRead ?? false;

      const url = `${API_BASE_URL}/api/notifications?page=${page}&pageSize=${pageSize}&includeRead=${includeRead}`;


      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json'
        }
      });

      

      if (!response.ok) {
        const errorText = await response.text();
        console.error('🔧 [NotificationService] Notification fetch error:', {
          status: response.status,
          statusText: response.statusText,
          errorText
        });
        throw new Error(`Failed to fetch notifications: ${response.statusText}. Details: ${errorText}`);
      }

      const data = await response.json();
      
      
      
      if (Array.isArray(data.notifications)) {
        data.notifications = data.notifications.map((notification: Notification) => ({
          ...notification,
          age: notification.age.split('.')[0].replace(/:\d{2}$/, '')
        }));
      } else {
        console.error('Unexpected notifications data format:', data);
        throw new Error('Invalid notifications data format received');
      }

      return data;
    } catch (error) {
      console.error('Error fetching notifications:', error);
      return {
        notifications: [],
        totalCount: 0,
        page: 1,
        pageSize: 20,
        totalPages: 0,
        message: error instanceof Error ? error.message : "Failed to fetch notifications"
      };
    }
  },

  async getUnreadCount(): Promise<GetUnreadCountResponse> {
    const token = getAuthToken();
    if (!token) {
      return { success: false, count: 0, message: "No authentication token found" };
    }

    const endpoint = `${API_BASE_URL}/api/notifications/count`;
    

    try {
      const response = await fetch(endpoint, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json'
        }
      });

      

      if (!response.ok) {
        const errorText = await response.text();
        console.error('🔧 [NotificationService] Unread count error:', {
          status: response.status,
          statusText: response.statusText,
          errorText
        });
        throw new Error(`Failed to get unread count: ${response.statusText}`);
      }

      const data = await response.json();
      
      return data;
    } catch (error) {
      console.error('🔧 [NotificationService] Error getting unread count:', error);
      
      return { success: false, count: 0, message: error instanceof Error ? error.message : "Failed to get unread count" };
    }
  },

  async markAsRead(notificationId: string): Promise<MarkAsReadResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/notifications/${notificationId}/read`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Accept': 'application/json'
      }
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to mark notification as read: ${errorText}`);
    }

    return response.json();
  },

  async markAllAsRead(): Promise<MarkAllAsReadResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/notifications/mark-all-read`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Accept': 'application/json'
      }
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to mark all notifications as read: ${errorText}`);
    }

    return response.json();
  },

  async deleteNotification(notificationId: string): Promise<DeleteNotificationResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/notifications/${notificationId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Accept': 'application/json'
      }
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Failed to delete notification: ${errorText}`);
    }

    return response.json();
  },

  async getPreferences(): Promise<GetNotificationPreferencesResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const endpoint = `${API_BASE_URL}/api/notifications/preferences`;
    

    try {
      const response = await fetch(endpoint, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json'
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('🔧 [NotificationService] Preferences fetch error:', {
          status: response.status,
          statusText: response.statusText,
          errorText
        });
        throw new Error(`Failed to get notification preferences: ${errorText}`);
      }

      const data = await response.json();

      
      
      if (data.success && data.preferences) {
        const transformedPreferences = {
          ...data.preferences,
          
          waterReminders: data.preferences.waterRemindersEnabled ?? data.preferences.waterReminders,
          mealReminders: data.preferences.mealRemindersEnabled ?? data.preferences.mealReminders,
          calorieWarnings: data.preferences.calorieLimitWarningsEnabled ?? data.preferences.calorieWarnings,
          goalAchievements: data.preferences.goalAchievementsEnabled ?? data.preferences.goalAchievements,
          weeklyProgress: data.preferences.weeklyProgressEnabled ?? data.preferences.weeklyProgress,
          dailySummary: data.preferences.dailySummaryEnabled ?? data.preferences.dailySummary,
          waterTime: data.preferences.waterReminderTime ?? data.preferences.waterTime,
          breakfastTime: data.preferences.breakfastReminderTime ?? data.preferences.breakfastTime,
          lunchTime: data.preferences.lunchReminderTime ?? data.preferences.lunchTime,
          dinnerTime: data.preferences.dinnerReminderTime ?? data.preferences.dinnerTime,
          waterFrequency: data.preferences.waterReminderFrequencyHours ?? data.preferences.waterFrequency,
          weekendNotifications: data.preferences.sendNotificationsOnWeekends ?? data.preferences.weekendNotifications,
        };
        

        return {
          ...data,
          preferences: transformedPreferences
        };
      }
      
      return data;
    } catch (error) {
      console.error('🔧 [NotificationService] Error fetching preferences:', error);
      throw error;
    }
  },

  async updatePreferences(preferences: UpdateNotificationPreferencesRequest): Promise<UpdateNotificationPreferencesResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }



    const endpoint = `${API_BASE_URL}/api/notifications/preferences`;


    try {
      const response = await fetch(endpoint, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(preferences)
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('🔧 [NotificationService] Preferences update error:', {
          status: response.status,
          statusText: response.statusText,
          errorText
        });
        throw new Error(`Failed to update notification preferences: ${errorText}`);
      }

      const data = await response.json();

      
      
      if (data.success && data.preferences) {
        const transformedPreferences = {
          ...data.preferences,
          
          waterReminders: data.preferences.waterRemindersEnabled ?? data.preferences.waterReminders,
          mealReminders: data.preferences.mealRemindersEnabled ?? data.preferences.mealReminders,
          calorieWarnings: data.preferences.calorieLimitWarningsEnabled ?? data.preferences.calorieWarnings,
          goalAchievements: data.preferences.goalAchievementsEnabled ?? data.preferences.goalAchievements,
          weeklyProgress: data.preferences.weeklyProgressEnabled ?? data.preferences.weeklyProgress,
          dailySummary: data.preferences.dailySummaryEnabled ?? data.preferences.dailySummary,
          waterTime: data.preferences.waterReminderTime ?? data.preferences.waterTime,
          breakfastTime: data.preferences.breakfastReminderTime ?? data.preferences.breakfastTime,
          lunchTime: data.preferences.lunchReminderTime ?? data.preferences.lunchTime,
          dinnerTime: data.preferences.dinnerReminderTime ?? data.preferences.dinnerTime,
          waterFrequency: data.preferences.waterReminderFrequencyHours ?? data.preferences.waterFrequency,
          weekendNotifications: data.preferences.sendNotificationsOnWeekends ?? data.preferences.weekendNotifications,
        };
        

        return {
          ...data,
          preferences: transformedPreferences
        };
      }
      
      return data;
    } catch (error) {
      console.error('🔧 [NotificationService] Error updating preferences:', error);
      throw error;
    }
  }
};

export default notificationService;