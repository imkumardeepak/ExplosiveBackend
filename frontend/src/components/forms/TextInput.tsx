import { forwardRef } from 'react';
import type { InputHTMLAttributes } from 'react';

interface TextInputProps extends InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
}

export const TextInput = forwardRef<HTMLInputElement, TextInputProps>(
  ({ error, className = '', ...props }, ref) => (
    <input
      ref={ref}
      className={`text-input ${error ? 'text-input--error' : ''} ${className}`}
      {...props}
    />
  ),
);

TextInput.displayName = 'TextInput';
