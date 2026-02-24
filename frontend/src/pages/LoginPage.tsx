import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema, type LoginFormValues } from '@/lib/schemas/auth.schema';
import { useLogin } from '@/hooks/useAuth';
import { FormField } from '@/components/forms/FormField';
import { TextInput } from '@/components/forms/TextInput';
import env from '@/lib/env';

const LoginPage = () => {
  const { mutate: login, isPending } = useLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { username: '', password: '' },
  });

  const onSubmit = (data: LoginFormValues) => login(data);

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-card__header">
          <h1 className="login-card__title">🏷️ {env.APP_NAME}</h1>
          <p className="login-card__subtitle">Sign in to your account</p>
        </div>
        <form onSubmit={handleSubmit(onSubmit)} className="login-form" noValidate>
          <FormField
            label="Username"
            error={errors.username?.message}
            required
            htmlFor="username"
          >
            <TextInput
              id="username"
              type="text"
              placeholder="Enter your username"
              error={!!errors.username}
              autoComplete="username"
              {...register('username')}
            />
          </FormField>
          <FormField
            label="Password"
            error={errors.password?.message}
            required
            htmlFor="password"
          >
            <TextInput
              id="password"
              type="password"
              placeholder="Enter your password"
              error={!!errors.password}
              autoComplete="current-password"
              {...register('password')}
            />
          </FormField>
          <button
            type="submit"
            className="login-form__submit"
            disabled={isPending}
          >
            {isPending ? 'Signing in...' : 'Sign In'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
