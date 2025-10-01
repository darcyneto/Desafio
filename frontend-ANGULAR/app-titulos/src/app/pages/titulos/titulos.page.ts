import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { TituloResumo } from '../../shared/models/titulo.model';
import { TituloService } from '../../shared/services/titulo.service';
import { CpfCnpjPipe } from '../../shared/pipes/cpf-cnpj.pipe';

@Component({
  standalone: true,
  templateUrl: './titulos.page.html',
  styleUrls: ['./titulos.page.css'],
  imports: [CommonModule, RouterModule, CpfCnpjPipe]
})
export class TitulosPage implements OnInit {
  titulos$!: Observable<TituloResumo[]>;

  constructor(private readonly servico: TituloService) {}

  ngOnInit(): void {
    this.titulos$ = this.servico.listar();
  }
}

