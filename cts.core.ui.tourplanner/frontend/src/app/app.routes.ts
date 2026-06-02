import { Routes } from '@angular/router';
import { Home } from './components/home/home';
import { Register } from './components/register/register';
import { Login } from './components/login/login';
import { TourDetail } from './components/tour-detail/tour-detail';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: 'register', component: Register, canActivate: [guestGuard] },
  { path: 'login', component: Login, canActivate: [guestGuard] },

  { path: 'tour/:tourGuid', component: TourDetail, canActivate: [authGuard] },
  { path: '', component: Home, canActivate: [authGuard] },

  { path: '**', redirectTo: '/login' },
];
