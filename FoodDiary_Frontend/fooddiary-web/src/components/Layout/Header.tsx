import React, { Fragment } from 'react';
import { Menu, Transition } from '@headlessui/react';
import { 
  Menu as MenuIcon, 
  User, 
  Settings, 
  LogOut,
  ChevronDown 
} from 'lucide-react';
import NotificationBell from '../UI/NotificationBell';
import { useAuthStore } from '../../stores/authStore';
import { cn } from '../../utils';

interface HeaderProps {
  onMenuClick: () => void;
}

const Header: React.FC<HeaderProps> = ({ onMenuClick }) => {
  const { user, logout } = useAuthStore();

  return (
    <header className="sticky top-0 z-40 bg-white border-b border-surface-200">
      <div className="flex items-center h-16 px-4 sm:px-6 lg:px-8">
        {/* Left side - Mobile menu button */}
        <div className="flex-shrink-0">
          <button
            type="button"
            className="lg:hidden p-2 rounded-md text-surface-400 hover:text-surface-500 hover:bg-surface-100"
            onClick={onMenuClick}
          >
            <MenuIcon className="h-6 w-6" />
          </button>
        </div>

        {/* Center - Title or Logo */}
        <div className="flex-grow"></div>

        {/* Right side */}
        <div className="flex items-center space-x-4 ml-auto">
          {/* Notifications */}
          <NotificationBell />

          {/* User menu */}
          <Menu as="div" className="relative">
            <Menu.Button className="flex items-center space-x-2 p-2 rounded-md text-surface-700 hover:text-surface-900 hover:bg-surface-100">
              <div className="w-8 h-8 bg-primary-600 rounded-full flex items-center justify-center">
                <span className="text-white text-sm font-medium">
                  {user?.name?.charAt(0).toUpperCase() || 'U'}
                </span>
              </div>
              <span className="hidden sm:block text-sm font-medium">
                {user?.name || 'User'}
              </span>
              <ChevronDown className="h-4 w-4" />
            </Menu.Button>

            <Transition
              as={Fragment}
              enter="transition ease-out duration-100"
              enterFrom="transform opacity-0 scale-95"
              enterTo="transform opacity-100 scale-100"
              leave="transition ease-in duration-75"
              leaveFrom="transform opacity-100 scale-100"
              leaveTo="transform opacity-0 scale-95"
            >
              <Menu.Items className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border border-surface-200 py-1 z-50">
                <Menu.Item>
                  {({ active }) => (
                    <a
                      href="/profile"
                      className={cn(
                        active ? 'bg-surface-100' : '',
                        'flex items-center px-4 py-2 text-sm text-surface-700'
                      )}
                    >
                      <User className="h-4 w-4 mr-3" />
                      Profile
                    </a>
                  )}
                </Menu.Item>
                
                <Menu.Item>
                  {({ active }) => (
                    <a
                      href="/settings"
                      className={cn(
                        active ? 'bg-surface-100' : '',
                        'flex items-center px-4 py-2 text-sm text-surface-700'
                      )}
                    >
                      <Settings className="h-4 w-4 mr-3" />
                      Settings
                    </a>
                  )}
                </Menu.Item>
                
                <Menu.Item>
                  {({ active }) => (
                    <button
                      onClick={logout}
                      className={cn(
                        active ? 'bg-surface-100' : '',
                        'flex items-center w-full px-4 py-2 text-sm text-surface-700'
                      )}
                    >
                      <LogOut className="h-4 w-4 mr-3" />
                      Sign out
                    </button>
                  )}
                </Menu.Item>
              </Menu.Items>
            </Transition>
          </Menu>
        </div>
      </div>
    </header>
  );
};

export default Header;
