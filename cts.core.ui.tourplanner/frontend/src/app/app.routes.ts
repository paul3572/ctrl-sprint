import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Register } from './register/register';
import { Login } from './login/login';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'register', component: Register },
  { path: 'login', component: Login },
  { path: '**', redirectTo: '' },
];
