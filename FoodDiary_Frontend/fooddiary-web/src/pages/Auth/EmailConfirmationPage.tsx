import React, { useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { CheckCircle, XCircle, Loader } from 'lucide-react';
import { authService } from '../../services/authService';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';

const EmailConfirmationPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('');

  const email = searchParams.get('email');
  const token = searchParams.get('token');

  useEffect(() => {
    const confirmEmail = async () => {
      if (!email || !token) {
        setStatus('error');
        setMessage('Invalid confirmation link. Please check your email and try again.');
        return;
      }

      try {
        await authService.confirmEmail(email, token);
        setStatus('success');
        setMessage('Your email has been confirmed successfully! You can now sign in to your account.');
      } catch (error: unknown) {
        setStatus('error');
        const errorMessage = error && typeof error === 'object' && 'response' in error && 
          error.response && typeof error.response === 'object' && 'data' in error.response &&
          error.response.data && typeof error.response.data === 'object' && 'message' in error.response.data
          ? (error.response.data as { message: string }).message 
          : 'Failed to confirm email. Please try again.';
        setMessage(errorMessage);
      }
    };

    confirmEmail();
  }, [email, token]);

  const getIcon = () => {
    switch (status) {
      case 'loading':
        return <Loader className="h-12 w-12 text-primary-600 animate-spin" />;
      case 'success':
        return <CheckCircle className="h-12 w-12 text-success-600" />;
      case 'error':
        return <XCircle className="h-12 w-12 text-error-600" />;
    }
  };

  const getTitle = () => {
    switch (status) {
      case 'loading':
        return 'Confirming your email...';
      case 'success':
        return 'Email confirmed!';
      case 'error':
        return 'Confirmation failed';
    }
  };

  return (
    <div className="min-h-screen bg-surface-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full">
        <Card>
          <div className="text-center space-y-6">
            {getIcon()}
            
            <div>
              <h1 className="text-2xl font-bold text-surface-900">
                {getTitle()}
              </h1>
              <p className="mt-2 text-sm text-surface-600">
                {message}
              </p>
            </div>

            {status === 'success' && (
              <div className="space-y-4">
                <Button asChild className="w-full">
                  <Link to="/login">
                    Sign in to your account
                  </Link>
                </Button>
              </div>
            )}

            {status === 'error' && (
              <div className="space-y-4">
                <Button asChild variant="outline" className="w-full">
                  <Link to="/login">
                    Back to sign in
                  </Link>
                </Button>
                <p className="text-xs text-surface-500">
                  If you're having trouble, please contact support or try registering again.
                </p>
              </div>
            )}
          </div>
        </Card>
      </div>
    </div>
  );
};

export default EmailConfirmationPage;
