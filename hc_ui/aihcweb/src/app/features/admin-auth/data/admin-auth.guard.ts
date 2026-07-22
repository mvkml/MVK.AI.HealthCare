import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AdminAuthMockService } from './admin-auth-mock.service';

export const adminAuthGuard: CanActivateFn = () => {
  const auth = inject(AdminAuthMockService);
  const router = inject(Router);

  if (auth.isLoggedIn()) {
    return true;
  }

  return router.createUrlTree(['/admin/login']);
};
