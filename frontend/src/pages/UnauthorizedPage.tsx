import { Link } from 'react-router-dom';

const UnauthorizedPage = () => (
  <div className="error-page">
    <h1 className="error-page__code">403</h1>
    <h2 className="error-page__title">Access Denied</h2>
    <p className="error-page__message">
      You do not have permission to access this page.
    </p>
    <Link to="/dashboard" className="error-page__link">
      Return to Dashboard
    </Link>
  </div>
);

export default UnauthorizedPage;
