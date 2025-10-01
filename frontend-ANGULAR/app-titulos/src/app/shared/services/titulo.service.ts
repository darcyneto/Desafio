import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TituloEntrada, TituloResumo } from '../models/titulo.model';

@Injectable({ providedIn: 'root' })
export class TituloService {
  private readonly baseUrl = `${environment.apiUrl}/api/titulos`;

  constructor(private readonly http: HttpClient) {}

  listar(): Observable<TituloResumo[]> {
    return this.http.get<TituloResumo[]>(this.baseUrl);
  }

  criar(entrada: TituloEntrada): Observable<TituloResumo> {
    console.log('🚀 Enviando requisição para:', this.baseUrl);
    console.log('📦 Payload:', entrada);
    return this.http.post<TituloResumo>(this.baseUrl, entrada);
  }
}

