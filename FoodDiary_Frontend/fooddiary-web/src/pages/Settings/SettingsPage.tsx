import React, { useState, useEffect } from 'react';
import { Settings, Bell, Calendar } from 'lucide-react';
import notificationService, { NotificationPreferences } from '../../services/notificationService';
import Card from '../../components/UI/Card';
import { toastService } from '../../services/toastService';

const defaultSettings: NotificationPreferences = {
  waterReminders: true,
  mealReminders: true,
  calorieWarnings: true,
  goalAchievements: true,
  weeklyProgress: true,
  dailySummary: true,
  waterFrequency: 2,
  weekendNotifications: true,
  waterTime: '09:00',
  breakfastTime: '08:00',
  lunchTime: '12:00',
  dinnerTime: '18:00',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString()
};

const SettingsPage: React.FC = () => {
  const [settings, setSettings] = useState<NotificationPreferences>(defaultSettings);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const loadSettings = async () => {
      try {
        setLoading(true);
        const response = await notificationService.getPreferences();
        if (response.success && response.preferences) {
          setSettings(response.preferences);
        } else {
          throw new Error(response.message || 'Failed to load preferences');
        }
      } catch (error) {
        console.error('Failed to load notification preferences:', error);
        toastService.error('Failed to load notification preferences');
      } finally {
        setLoading(false);
      }
    };

    loadSettings();
  }, []);

  const handleSaveSettings = async () => {
    try {
      setSaving(true);
      const response = await notificationService.updatePreferences(settings);
      if (response.success && response.preferences) {
        setSettings(response.preferences);
        toastService.success('Settings saved successfully');
      } else {
        throw new Error(response.message || 'Failed to save settings');
      }
    } catch (error) {
      console.error('Failed to save settings:', error);
      toastService.error('Failed to save settings');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="text-center py-8">Loading settings...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-surface-900">Settings</h1>
        <p className="text-surface-600">Manage your account and application preferences.</p>
      </div>

      <Card className="p-6">
        <div className="flex items-center mb-6">
          <h2 className="text-xl font-semibold flex items-center gap-2">
            <Bell className="h-6 w-6" />
            Notification Settings
          </h2>
        </div>

        <div className="space-y-6">
          {/* Notification Types */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium flex items-center gap-2">
              <Bell className="h-5 w-5" />
              Notification Types
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {[
                { key: 'waterReminders', label: 'Water Reminders', icon: '💧' },
                { key: 'mealReminders', label: 'Meal Reminders', icon: '🍽️' },
                { key: 'calorieWarnings', label: 'Calorie Warnings', icon: '⚠️' },
                { key: 'goalAchievements', label: 'Goal Achievements', icon: '🎉' },
                { key: 'weeklyProgress', label: 'Weekly Progress', icon: '📊' },
                { key: 'dailySummary', label: 'Daily Summary', icon: '📋' }
              ].map(({ key, label, icon }) => (
                <div key={key} className="flex items-center gap-3 p-3 border rounded-lg">
                  <span className="text-2xl">{icon}</span>
                  <div className="flex-1">
                    <label className="font-medium">{label}</label>
                  </div>
                  <input
                    type="checkbox"
                    checked={!!(settings[key as keyof NotificationPreferences] as boolean)}
                    onChange={(e) => setSettings({ ...settings, [key]: e.target.checked })}
                    className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                </div>
              ))}
            </div>
          </div>

          {/* Timing Preferences */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium flex items-center gap-2">
              <Calendar className="h-5 w-5" />
              Timing Preferences
            </h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {[
                { key: 'waterTime', label: 'Water Reminder Time', icon: '💧' },
                { key: 'breakfastTime', label: 'Breakfast Time', icon: '🌅' },
                { key: 'lunchTime', label: 'Lunch Time', icon: '☀️' },
                { key: 'dinnerTime', label: 'Dinner Time', icon: '🌙' }
              ].map(({ key, label, icon }) => (
                <div key={key} className="flex items-center gap-3 p-3 border rounded-lg">
                  <span className="text-2xl">{icon}</span>
                  <div className="flex-1">
                    <label className="font-medium">{label}</label>
                  </div>
                  <input
                    type="time"
                    value={settings[key as keyof NotificationPreferences] as string || ''}
                    onChange={(e) => setSettings({ ...settings, [key]: e.target.value })}
                    className="border border-gray-300 rounded px-3 py-2"
                  />
                </div>
              ))}
            </div>
          </div>

          {/* Frequency & Behavior */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium flex items-center gap-2">
              <Settings className="h-5 w-5" />
              Frequency & Behavior
            </h3>
            
            <div className="space-y-4">
              <div className="flex items-center gap-3 p-3 border rounded-lg">
                <span className="text-2xl">⏰</span>
                <div className="flex-1">
                  <label className="font-medium">Water Reminder Frequency (hours)</label>
                </div>
                <select
                  value={settings.waterFrequency || 2}
                  onChange={(e) => setSettings({ ...settings, waterFrequency: parseInt(e.target.value) })}
                  className="border border-gray-300 rounded px-3 py-2"
                >
                  <option value={1}>Every 1 hour</option>
                  <option value={2}>Every 2 hours</option>
                  <option value={3}>Every 3 hours</option>
                  <option value={4}>Every 4 hours</option>
                  <option value={6}>Every 6 hours</option>
                </select>
              </div>
              
              <div className="flex items-center gap-3 p-3 border rounded-lg">
                <span className="text-2xl">📅</span>
                <div className="flex-1">
                  <label className="font-medium">Send notifications on weekends</label>
                </div>
                <input
                  type="checkbox"
                  checked={!!settings.weekendNotifications}
                  onChange={(e) => setSettings({ ...settings, weekendNotifications: e.target.checked })}
                  className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
              </div>
            </div>
          </div>

          {/* Save Button */}
          <div className="pt-4 border-t">
            <button
              onClick={handleSaveSettings}
              disabled={saving}
              className="px-6 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50"
            >
              {saving ? 'Saving...' : 'Save Settings'}
            </button>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default SettingsPage;