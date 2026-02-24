import type { ReactNode } from 'react';

interface FormFieldProps {
  label: string;
  error?: string;
  required?: boolean;
  children: ReactNode;
  htmlFor?: string;
}

export const FormField = ({ label, error, required, children, htmlFor }: FormFieldProps) => (
  <div className="form-field">
    <label className="form-field__label" htmlFor={htmlFor}>
      {label}
      {required && <span className="form-field__required">*</span>}
    </label>
    {children}
    {error && (
      <span className="form-field__error" role="alert">
        {error}
      </span>
    )}
  </div>
);
