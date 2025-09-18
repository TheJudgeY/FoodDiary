import React, { useState, useEffect, useCallback } from 'react';
import { Bell } from 'lucide-react';
import notificationService, { Notification } from '../../services/notificationService';
import { useNotificationRouter } from '../../hooks/useNotificationRouter';
import { toastService } from '../../services/toastService';

const NotificationBell: React.FC = () => {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const { routeNotification } = useNotificationRouter();

  const fetchNotifications = useCallback(async () => {
    try {
      setIsLoading(true);
      const response = await notificationService.getNotifications({
        page: 1,
        pageSize: 20,
        includeRead: false
      });
      
      if (Array.isArray(response.notifications)) {
        setNotifications(response.notifications);
      }

      const unreadResponse = await notificationService.getUnreadCount();
      if (unreadResponse.success) {
        setUnreadCount(unreadResponse.count);
      }
    } catch (error) {
      console.error('Error fetching notifications:', error);
      toastService.error('Failed to fetch notifications');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  const handleNotificationClick = async (notification: Notification) => {
    try {
      const response = await notificationService.markAsRead(notification.id);
      if (response.success) {
        setNotifications(prev => 
          prev.map(n => n.id === notification.id ? { ...n, isRead: true, isUnread: false } : n)
        );
        setUnreadCount(prev => Math.max(0, prev - 1));
        
        
        if (notification.type === 'GoalAchievement') {
          
          window.location.href = '/analytics?view=goals&period=week';
        } else if (notification.type !== 'CalorieLimitWarning') {
          
          routeNotification(notification.contextData);
        }
        
        setIsDropdownOpen(false);
      }
    } catch (error) {
      console.error('Error marking notification as read:', error);
      toastService.error('Failed to mark notification as read');
    }
  };

  const handleDelete = async (e: React.MouseEvent, notification: Notification) => {
    e.stopPropagation();
    try {
      const response = await notificationService.deleteNotification(notification.id);
      if (response.success) {
        setNotifications(prev => prev.filter(n => n.id !== notification.id));
        if (notification.isUnread) {
          setUnreadCount(prev => Math.max(0, prev - 1));
        }
        toastService.success('Notification deleted');
      }
    } catch (error) {
      console.error('Error deleting notification:', error);
      toastService.error('Failed to delete notification');
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      const response = await notificationService.markAllAsRead();
      if (response.success) {
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true, isUnread: false })));
        setUnreadCount(0);
        toastService.success('All notifications marked as read');
      }
    } catch (error) {
      console.error('Error marking all notifications as read:', error);
      toastService.error('Failed to mark all notifications as read');
    }
  };

  return (
    <div className="relative">
      <button
        className="relative p-2 text-gray-600 hover:text-gray-800 focus:outline-none"
        onClick={() => setIsDropdownOpen(!isDropdownOpen)}
      >
        <Bell className="h-6 w-6" />
        {unreadCount > 0 && (
          <span className="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-red-500 rounded-full">
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
      </button>

      {isDropdownOpen && (
        <div className="absolute right-0 mt-2 w-80 sm:w-96 bg-white rounded-lg shadow-xl z-50 max-h-[80vh] overflow-y-auto border">
          <div className="p-3 sm:p-4 border-b">
            <div className="flex justify-between items-center">
              <h3 className="text-base sm:text-lg font-semibold">Notifications</h3>
              {unreadCount > 0 && (
                <button
                  onClick={handleMarkAllAsRead}
                  className="text-xs sm:text-sm text-blue-600 hover:text-blue-800"
                >
                  Mark all as read
                </button>
              )}
            </div>
          </div>

          {isLoading ? (
            <div className="p-4 text-center">Loading...</div>
          ) : notifications.length === 0 ? (
            <div className="p-4 text-center text-gray-500">No notifications</div>
          ) : (
            <div>
              {notifications.map((notification) => (
                <div
                  key={notification.id}
                  onClick={() => handleNotificationClick(notification)}
                  className={`p-3 sm:p-4 border-b cursor-pointer hover:bg-gray-50 ${
                    notification.isUnread ? 'bg-blue-50' : ''
                  }`}
                >
                  <div className="flex justify-between items-start gap-2">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="text-lg sm:text-xl flex-shrink-0">{notification.typeIcon}</span>
                        <h4 className="font-semibold text-sm sm:text-base truncate">{notification.title}</h4>
                      </div>
                      <p className="text-xs sm:text-sm text-gray-600 mb-2 line-clamp-2 leading-relaxed">
                        {notification.message}
                      </p>
                      <div className="flex items-center gap-2 text-xs text-gray-500">
                        <span className="flex-shrink-0">{notification.age}</span>
                        <span className="flex-shrink-0">•</span>
                        <span
                          className={`px-2 py-1 rounded-full text-xs flex-shrink-0 ${
                            notification.priorityColor === 'red'
                              ? 'bg-red-100 text-red-800'
                              : notification.priorityColor === 'yellow'
                              ? 'bg-yellow-100 text-yellow-800'
                              : 'bg-blue-100 text-blue-800'
                          }`}
                        >
                          {notification.priority}
                        </span>
                      </div>
                    </div>
                    <button
                      onClick={(e) => handleDelete(e, notification)}
                      className="text-gray-400 hover:text-gray-600 flex-shrink-0 p-1"
                      title="Delete notification"
                    >
                      ×
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default NotificationBell;