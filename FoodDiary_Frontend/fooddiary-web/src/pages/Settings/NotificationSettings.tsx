import React, { useEffect } from 'react';
import { useNotificationStore } from '../../stores/notificationStore';
import { NotificationPreferences } from '../../services/notificationService';
import Card from '../../components/UI/Card';

const NotificationSettings: React.FC = () => {
  const {
    preferences,
    isPreferencesLoading,
    preferencesError,
    fetchPreferences,
    updatePreferences
  } = useNotificationStore();

  useEffect(() => {
    fetchPreferences();
  }, [fetchPreferences]);

  const handleToggle = (key: keyof NotificationPreferences) => {
    if (!preferences) return;
    updatePreferences({ [key]: !preferences[key] });
  };



  if (isPreferencesLoading) {
    return <div>Loading preferences...</div>;
  }

  if (preferencesError) {
    return <div>Error loading preferences: {preferencesError}</div>;
  }

  if (!preferences) {
    return <div>No preferences found</div>;
  }

  return (
    <Card className="p-4 sm:p-6">
      <div className="mb-4 sm:mb-6">
        <h2 className="text-lg sm:text-xl font-semibold">Notification Settings</h2>
      </div>

      <div className="space-y-4 sm:space-y-6">
        {/* Notification Types */}
        <div className="space-y-3 sm:space-y-4">
          <h3 className="text-base sm:text-lg font-medium">Notification Types</h3>
          <div className="grid grid-cols-1 gap-3 sm:gap-4">
            {[
              { key: 'waterReminders', label: 'Water Reminders', icon: '💧' },
              { key: 'mealReminders', label: 'Meal Reminders', icon: '🍽️' },
              { key: 'calorieWarnings', label: 'Calorie Warnings', icon: '⚠️' },
              { key: 'goalAchievements', label: 'Goal Achievements', icon: '🎉' },
              { key: 'weeklyProgress', label: 'Weekly Progress', icon: '📊' },
              { key: 'dailySummary', label: 'Daily Summary', icon: '📋' }
            ].map(({ key, label, icon }) => (
              <div key={key} className="flex items-center gap-3 p-3 border rounded-lg">
                <span className="text-xl sm:text-2xl">{icon}</span>
                <div className="flex-1">
                  <label className="font-medium text-sm sm:text-base">{label}</label>
                </div>
                <input
                  type="checkbox"
                  checked={!!(preferences[key as keyof NotificationPreferences] as boolean)}
                  onChange={() => handleToggle(key as keyof NotificationPreferences)}
                  className="rounded border-gray-300 text-blue-600 focus:ring-blue-500 w-4 h-4 sm:w-5 sm:h-5"
                />
              </div>
            ))}
          </div>
        </div>

        {/* Timing Preferences */}
        <div className="space-y-3 sm:space-y-4">
          <h3 className="text-base sm:text-lg font-medium">Timing Preferences</h3>
          <div className="grid grid-cols-1 gap-3 sm:gap-4">
            {[
              { key: 'waterTime', label: 'Water Reminder Time', icon: '💧' },
              { key: 'breakfastTime', label: 'Breakfast Time', icon: '🌅' },
              { key: 'lunchTime', label: 'Lunch Time', icon: '☀️' },
              { key: 'dinnerTime', label: 'Dinner Time', icon: '🌙' }
            ].map(({ key, label, icon }) => (
              <div key={key} className="flex flex-col sm:flex-row sm:items-center gap-3 p-3 border rounded-lg">
                <div className="flex items-center gap-3">
                  <span className="text-xl sm:text-2xl">{icon}</span>
                  <label className="font-medium text-sm sm:text-base">{label}</label>
                </div>
                <input
                  type="time"
                  value={preferences[key as keyof NotificationPreferences] as string || ''}
                  onChange={(e) => updatePreferences({ [key]: e.target.value })}
                  className="border border-gray-300 rounded px-3 py-2 w-full sm:w-auto"
                />
              </div>
            ))}
          </div>
        </div>

        {/* Additional Settings */}
        <div className="space-y-3 sm:space-y-4">
          <h3 className="text-base sm:text-lg font-medium">Additional Settings</h3>
          <div className="space-y-3 sm:space-y-4">
            <div className="flex flex-col sm:flex-row sm:items-center gap-3 p-3 border rounded-lg">
              <div className="flex items-center gap-3">
                <span className="text-xl sm:text-2xl">⏰</span>
                <label className="font-medium text-sm sm:text-base">Water Reminder Frequency (hours)</label>
              </div>
              <select
                value={preferences.waterFrequency || 2}
                onChange={(e) => updatePreferences({ waterFrequency: parseInt(e.target.value) })}
                className="border border-gray-300 rounded px-3 py-2 w-full sm:w-auto"
              >
                <option value={1}>Every 1 hour</option>
                <option value={2}>Every 2 hours</option>
                <option value={3}>Every 3 hours</option>
                <option value={4}>Every 4 hours</option>
                <option value={6}>Every 6 hours</option>
              </select>
            </div>

            <div className="flex items-center gap-3 p-3 border rounded-lg">
              <span className="text-xl sm:text-2xl">📅</span>
              <div className="flex-1">
                <label className="font-medium text-sm sm:text-base">Send notifications on weekends</label>
              </div>
              <input
                type="checkbox"
                checked={!!preferences.weekendNotifications}
                onChange={() => handleToggle('weekendNotifications')}
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500 w-4 h-4 sm:w-5 sm:h-5"
              />
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
};

export default NotificationSettings;