import { Directive, ElementRef, HostListener, Input } from '@angular/core';
import { MaskService } from '../services/mask.service';

@Directive({
  selector: '[appCpfCnpjMask]',
  standalone: true
})
export class CpfCnpjMaskDirective {
  
  constructor(
    private el: ElementRef,
    private maskService: MaskService
  ) {}

  @HostListener('input', ['$event'])
  onInput(event: any): void {
    const value = event.target.value;
    const maskedValue = this.maskService.cpfCnpjMask(value);
    event.target.value = maskedValue;
  }

  @HostListener('blur', ['$event'])
  onBlur(event: any): void {
    const value = event.target.value;
    const maskedValue = this.maskService.cpfCnpjMask(value);
    event.target.value = maskedValue;
  }
}
