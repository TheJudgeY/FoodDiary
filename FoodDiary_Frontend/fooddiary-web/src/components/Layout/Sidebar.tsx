import React from 'react';
import { NavLink } from 'react-router-dom';
import { UI_ICONS } from '../../constants/icons';
import { cn } from '../../utils';

const navigation = [
  { name: 'Dashboard', href: '/dashboard', icon: UI_ICONS.home },
  { name: 'Food Diary', href: '/diary', icon: UI_ICONS.calendar },
  { name: 'Products', href: '/products', icon: UI_ICONS.package },
  { name: 'Recipes', href: '/recipes', icon: UI_ICONS.chefHat },
  { name: 'Analytics', href: '/analytics', icon: UI_ICONS.barChart },
  { name: 'Profile', href: '/profile', icon: UI_ICONS.user },
  { name: 'Settings', href: '/settings', icon: UI_ICONS.settings },
];

const Sidebar: React.FC = () => {
  return (
    <div className="hidden lg:fixed lg:inset-y-0 lg:z-50 lg:flex lg:w-72 lg:flex-col">
      <div className="flex grow flex-col gap-y-5 overflow-y-auto bg-white border-r border-surface-200 px-6 pb-4">
        {/* Logo */}
        <div className="flex h-16 shrink-0 items-center">
          <h1 className="text-xl font-bold text-primary-600">FoodDiary</h1>
        </div>

        {/* Navigation */}
        <nav className="flex flex-1 flex-col">
          <ul role="list" className="flex flex-1 flex-col gap-y-7">
            <li>
              <ul role="list" className="-mx-2 space-y-1">
                {navigation.map((item) => (
                  <li key={item.name}>
                    <NavLink
                      to={item.href}
                      className={({ isActive }) =>
                        cn(
                          isActive
                            ? 'bg-primary-50 text-primary-600'
                            : 'text-surface-700 hover:text-primary-600 hover:bg-primary-50',
                          'group flex gap-x-3 rounded-md p-2 text-sm leading-6 font-medium'
                        )
                      }
                    >
                      <item.icon className="h-6 w-6 shrink-0" />
                      {item.name}
                    </NavLink>
                  </li>
                ))}
              </ul>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  );
};

export default Sidebar;