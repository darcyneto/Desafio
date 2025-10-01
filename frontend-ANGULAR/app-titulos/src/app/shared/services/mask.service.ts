import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MaskService {

  /**
   * Aplica máscara dinâmica para CPF (11 dígitos) ou CNPJ (14 dígitos)
   */
  cpfCnpjMask(value: string): string {
    if (!value) return '';
    
    // Remove todos os caracteres não numéricos
    const numbers = value.replace(/\D/g, '');
    
    if (numbers.length <= 11) {
      // Máscara de CPF: 000.000.000-00
      return numbers
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else {
      // Máscara de CNPJ: 00.000.000/0000-00
      return numbers
        .replace(/(\d{2})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1/$2')
        .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
    }
  }

  /**
   * Aplica máscara de porcentagem com 2 casas decimais
   */
  percentageMask(value: string): string {
    if (!value) return '';
    
    // Remove todos os caracteres não numéricos exceto vírgula
    let numbers = value.replace(/[^\d,]/g, '');
    
    // Substitui vírgula por ponto para processamento
    numbers = numbers.replace(',', '.');
    
    // Converte para número e limita a 2 casas decimais
    const num = parseFloat(numbers);
    if (isNaN(num)) return '';
    
    // Formata com 2 casas decimais
    return num.toFixed(2);
  }

  /**
   * Aplica máscara de moeda brasileira (BRL)
   */
  currencyMask(value: string): string {
    if (!value) return '';
    
    // Remove todos os caracteres não numéricos exceto vírgula
    let numbers = value.replace(/[^\d,]/g, '');
    
    // Substitui vírgula por ponto para processamento
    numbers = numbers.replace(',', '.');
    
    // Converte para número
    const num = parseFloat(numbers);
    if (isNaN(num)) return '';
    
    // Formata como moeda brasileira
    return new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(num);
  }

  /**
   * Remove máscara e retorna apenas números
   */
  removeMask(value: string): string {
    return value.replace(/\D/g, '');
  }

  /**
   * Remove máscara de moeda e retorna número
   */
  removeCurrencyMask(value: string): number {
    const cleanValue = value.replace(/[^\d,]/g, '').replace(',', '.');
    return parseFloat(cleanValue) || 0;
  }

  /**
   * Remove máscara de porcentagem e retorna número
   */
  removePercentageMask(value: string): number {
    const cleanValue = value.replace(/[^\d,]/g, '').replace(',', '.');
    return parseFloat(cleanValue) || 0;
  }
}
