import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

interface NotificationContext {
  action: 'navigate' | 'show' | 'open';
  target: string;
  mealType?: string;
  showAchievement?: boolean;
  view?: string;
  period?: string;
  tab?: string;
  section?: string;
}

export const useNotificationRouter = () => {
  const navigate = useNavigate();

  const routeNotification = useCallback((contextData: string | null) => {
    if (!contextData) {
      return;
    }

    try {
      const context = JSON.parse(contextData) as NotificationContext;
      
      switch (context.action) {
        case 'navigate':
          switch (context.target) {
            case 'food-entry':
              navigate(`/food-entry?mealType=${context.mealType}`);
              break;
            case 'goals':
              navigate('/goals', { state: { showAchievement: context.showAchievement } });
              break;
            case 'analytics':
              navigate(`/analytics?view=${context.view}&period=${context.period}`);
              break;
            case 'dashboard':
              navigate(`/dashboard?tab=${context.tab}&section=${context.section}`);
              break;
            case 'goal-achievement':
              
              navigate('/analytics?view=goals&period=week');
              break;
            default:
              navigate('/dashboard');
          }
          break;
        case 'show':
          
          break;
        case 'open':
          
          break;
        default:
          
      }
    } catch (error) {
      console.error('Failed to parse notification context:', error);
      
    }
  }, [navigate]);

  return { routeNotification };
};

export default useNotificationRouter;