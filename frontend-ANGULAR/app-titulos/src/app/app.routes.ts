import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'titulos', pathMatch: 'full' },
  {
    path: 'titulos',
    loadComponent: () => import('./pages/titulos/titulos.page').then(m => m.TitulosPage)
  },
  {
    path: 'titulos/novo',
    loadComponent: () => import('./pages/novo-titulo/novo-titulo.page').then(m => m.NovoTituloPage)
  },
  { path: '**', redirectTo: 'titulos' }
];

