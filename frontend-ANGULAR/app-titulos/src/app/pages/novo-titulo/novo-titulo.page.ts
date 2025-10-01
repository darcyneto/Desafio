import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TituloService } from '../../shared/services/titulo.service';
import { CpfCnpjMaskDirective } from '../../shared/directives/cpf-cnpj-mask.directive';
import { MaskService } from '../../shared/services/mask.service';

@Component({
  standalone: true,
  templateUrl: './novo-titulo.page.html',
  styleUrls: ['./novo-titulo.page.css'],
  imports: [CommonModule, ReactiveFormsModule, RouterModule, CpfCnpjMaskDirective]
})
export class NovoTituloPage {
  formulario: FormGroup;
  mensagem = '';

  constructor(
    private readonly fb: FormBuilder, 
    private readonly servico: TituloService, 
    private readonly router: Router,
    private readonly maskService: MaskService
  ) {
    this.formulario = this.fb.group({
      numeroTitulo: ['', Validators.required],
      nomeDevedor: ['', Validators.required],
      cpfDevedor: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(18)]],
      jurosAoMes: [0, [Validators.required, Validators.min(0)]],
      multaPercentual: [0, [Validators.required, Validators.min(0)]],
      parcelas: this.fb.array([this.criarParcela()])
    });
  }

  get parcelas(): FormArray {
    return this.formulario.get('parcelas') as FormArray;
  }

  adicionarParcela(): void {
    this.parcelas.push(this.criarParcela());
  }

  removerParcela(indice: number): void {
    if (this.parcelas.length > 1) {
      this.parcelas.removeAt(indice);
    }
  }

  salvar(): void {
    console.log('🔍 Iniciando processo de salvamento...');
    console.log('📝 Formulário válido:', this.formulario.valid);
    console.log('📋 Valores do formulário:', this.formulario.value);
    console.log('🔧 Status dos controles:', this.formulario.status);
    
    // Marcar todos os campos como tocados para mostrar erros de validação
    this.formulario.markAllAsTouched();
    
    if (this.formulario.invalid) {
      this.mensagem = 'Preencha todos os campos obrigatórios.';
      console.log('❌ Formulário inválido:', this.formulario.errors);
      console.log('❌ Erros nos controles:', this.getFormErrors());
      
      // Temporariamente, vamos forçar o envio para testar
      console.log('🧪 TESTE: Forçando envio mesmo com formulário inválido...');
    }

    const valores = this.formulario.value;
    const payload = {
      numeroTitulo: valores.numeroTitulo,
      nomeDevedor: valores.nomeDevedor,
      cpfDevedor: this.maskService.removeMask(valores.cpfDevedor), // Remove máscara do CPF/CNPJ
      jurosAoMes: this.converterPercentualParaDecimal(valores.jurosAoMes),
      multaPercentual: this.converterPercentualParaDecimal(valores.multaPercentual),
      parcelas: valores.parcelas.map((parcela: { numero: number; vencimento: string; valor: number }) => ({
        numero: parcela.numero,
        vencimento: parcela.vencimento,
        valor: Number(parcela.valor)
      }))
    };

    console.log('🚀 Chamando serviço para criar título...');
    console.log('📡 URL base do serviço:', this.servico['baseUrl']);
    
    this.servico.criar(payload).subscribe({
      next: (resultado) => {
        console.log('✅ Título criado com sucesso:', resultado);
        this.mensagem = 'Título cadastrado com sucesso!';
        this.router.navigate(['/titulos']);
      },
      error: (error) => {
        console.error('❌ Erro ao cadastrar título:', error);
        console.error('❌ Status:', error.status);
        console.error('❌ Mensagem:', error.message);
        console.error('❌ Error completo:', error);
        this.mensagem = `Erro ao cadastrar título: ${error.message || 'Erro desconhecido'}`;
      }
    });
  }

  private criarParcela(): FormGroup {
    return this.fb.group({
      numero: [1, [Validators.required, Validators.min(1)]],
      vencimento: ['', Validators.required],
      valor: [0.01, [Validators.required, Validators.min(0.01)]] // Valor inicial como número
    });
  }

  private converterPercentualParaDecimal(valor: number): number {
    return Number((valor / 100).toFixed(4));
  }

  private getFormErrors(): any {
    const errors: any = {};
    Object.keys(this.formulario.controls).forEach(key => {
      const control = this.formulario.get(key);
      if (control && control.errors) {
        errors[key] = control.errors;
      }
    });
    return errors;
  }
}

