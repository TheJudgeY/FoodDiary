import React from 'react';
import { cn } from '../../utils';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  padding?: 'none' | 'sm' | 'md' | 'lg';
  elevation?: 'none' | 'sm' | 'md' | 'lg';
}

const Card: React.FC<CardProps> = ({
  children,
  className,
  padding = 'md',
  elevation = 'sm',
}) => {
  const paddingClasses = {
    none: '',
    sm: 'p-4',
    md: 'p-6',
    lg: 'p-8',
  };

  const elevationClasses = {
    none: 'border border-surface-200',
    sm: 'shadow-sm border border-surface-200',
    md: 'shadow-md border border-surface-200',
    lg: 'shadow-lg border border-surface-200',
  };

  return (
    <div
      className={cn(
        'rounded-xl bg-white',
        paddingClasses[padding],
        elevationClasses[elevation],
        className
      )}
    >
      {children}
    </div>
  );
};

export default Card;
