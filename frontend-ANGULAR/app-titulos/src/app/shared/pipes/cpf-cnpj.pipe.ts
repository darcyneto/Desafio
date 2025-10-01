import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'cpfCnpj',
  standalone: true
})
export class CpfCnpjPipe implements PipeTransform {

  transform(value: string): string {
    if (!value) return '';

    // Remove todos os caracteres não numéricos
    const numbers = value.replace(/\D/g, '');

    if (numbers.length === 11) {
      // Máscara de CPF: 000.000.000-00
      return numbers
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    } else if (numbers.length === 14) {
      // Máscara de CNPJ: 00.000.000/0000-00
      return numbers
        .replace(/(\d{2})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1/$2')
        .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
    } else {
      // Se não for 11 ou 14 dígitos, retorna o valor original
      return value;
    }
  }
}
