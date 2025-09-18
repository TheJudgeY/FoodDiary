import { API_BASE_URL } from '../constants';
import { getAuthToken } from '../utils';

export interface TimezoneResponse {
  success: boolean;
  timeZoneId: string;
  message: string;
}

const timezoneService = {
  
  getCurrentTimezone: (): string => {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
  },

  
  updateTimezone: async (timeZoneId: string): Promise<TimezoneResponse> => {
    const token = getAuthToken();
    if (!token) throw new Error('Authentication required');

    try {
      const response = await fetch(`${API_BASE_URL}/api/users/timezone`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ timeZoneId })
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Failed to update timezone');
      }

      return response.json();
    } catch (error) {
      console.error('Error in updateTimezone:', error);
      throw error;
    }
  },

  
  getAvailableTimezones: () => [
    { id: 'America/New_York', name: 'Eastern Time', offset: 'UTC-5/UTC-4' },
    { id: 'America/Chicago', name: 'Central Time', offset: 'UTC-6/UTC-5' },
    { id: 'America/Denver', name: 'Mountain Time', offset: 'UTC-7/UTC-6' },
    { id: 'America/Los_Angeles', name: 'Pacific Time', offset: 'UTC-8/UTC-7' },
    { id: 'Europe/London', name: 'British Time', offset: 'UTC+0/UTC+1' },
    { id: 'Europe/Paris', name: 'Central European Time', offset: 'UTC+1/UTC+2' },
    { id: 'Asia/Tokyo', name: 'Japan Time', offset: 'UTC+9' },
    { id: 'Australia/Sydney', name: 'Australian Eastern Time', offset: 'UTC+10/UTC+11' },
    { id: 'UTC', name: 'Universal Coordinated Time', offset: 'UTC+0' }
  ]
};

export default timezoneService;
