import { Routes } from '@angular/router';
import { authGuard } from './features/auth/data/auth.guard';
import { adminAuthGuard } from './features/admin-auth/data/admin-auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/pages/login/login').then((m) => m.Login)
  },
  {
    path: 'signup',
    loadComponent: () => import('./features/auth/pages/signup/signup').then((m) => m.Signup)
  },
  {
    path: 'admin/login',
    loadComponent: () =>
      import('./features/admin-auth/pages/admin-login/admin-login').then((m) => m.AdminLogin)
  },
  {
    path: 'admin/signup',
    loadComponent: () =>
      import('./features/admin-auth/pages/admin-signup/admin-signup').then((m) => m.AdminSignup)
  },
  {
    path: 'admin/home',
    loadComponent: () =>
      import('./features/admin-auth/pages/admin-home/admin-home').then((m) => m.AdminHome),
    canActivate: [adminAuthGuard]
  },
  {
    path: 'home',
    loadComponent: () => import('./features/home/pages/home/home').then((m) => m.Home),
    canActivate: [authGuard]
  },
  {
    path: 'chat',
    loadChildren: () => import('./features/chat/chat.routes').then((m) => m.CHAT_ROUTES),
    canActivate: [authGuard]
  },
  {
    path: 'patient-chat',
    loadComponent: () =>
      import('./features/patient-chat/pages/patient-chat-page/patient-chat-page').then(
        (m) => m.PatientChatPage
      ),
    canActivate: [authGuard]
  }
];
