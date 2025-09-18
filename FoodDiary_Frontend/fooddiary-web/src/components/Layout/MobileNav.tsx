import React, { Fragment } from 'react';
import { Dialog, Transition } from '@headlessui/react';
import { NavLink } from 'react-router-dom';
import { cn } from '../../utils';

const navigation = [
  { name: 'Dashboard', href: '/dashboard' },
  { name: 'Food Diary', href: '/diary' },
  { name: 'Products', href: '/products' },
  { name: 'Recipes', href: '/recipes' },
  { name: 'Analytics', href: '/analytics' },
  { name: 'Profile', href: '/profile' },
  { name: 'Settings', href: '/settings' },
];

interface MobileNavProps {
  open: boolean;
  setOpen: (open: boolean) => void;
}

const MobileNav: React.FC<MobileNavProps> = ({ open, setOpen }) => {
  return (
    <Transition.Root show={open} as={Fragment}>
      <Dialog as="div" className="relative z-50 lg:hidden" onClose={setOpen}>
        <Transition.Child
          as={Fragment}
          enter="transition-opacity ease-linear duration-300"
          enterFrom="opacity-0"
          enterTo="opacity-100"
          leave="transition-opacity ease-linear duration-300"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
        >
          <div className="fixed inset-0 bg-surface-900/80" />
        </Transition.Child>

        <div className="fixed inset-0 flex">
          <Transition.Child
            as={Fragment}
            enter="transition ease-in-out duration-300 transform"
            enterFrom="-translate-x-full"
            enterTo="translate-x-0"
            leave="transition ease-in-out duration-300 transform"
            leaveFrom="translate-x-0"
            leaveTo="-translate-x-full"
          >
            <Dialog.Panel className="relative mr-16 flex w-full max-w-xs flex-1">
              <div className="flex grow flex-col gap-y-5 overflow-y-auto bg-white px-6 pb-4">
                <div className="flex h-16 shrink-0 items-center">
                  <h1 className="text-xl font-bold text-primary-600">FoodDiary</h1>
                </div>
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
                              onClick={() => setOpen(false)}
                            >
                              {item.name}
                            </NavLink>
                          </li>
                        ))}
                      </ul>
                    </li>
                  </ul>
                </nav>
              </div>
            </Dialog.Panel>
          </Transition.Child>
        </div>
      </Dialog>
    </Transition.Root>
  );
};

export default MobileNav;