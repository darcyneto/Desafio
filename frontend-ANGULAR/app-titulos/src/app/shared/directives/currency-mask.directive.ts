import { Directive, ElementRef, HostListener } from '@angular/core';
import { MaskService } from '../services/mask.service';

@Directive({
  selector: '[appCurrencyMask]',
  standalone: true
})
export class CurrencyMaskDirective {
  
  constructor(
    private el: ElementRef,
    private maskService: MaskService
  ) {}

  @HostListener('input', ['$event'])
  onInput(event: any): void {
    const value = event.target.value;
    const maskedValue = this.maskService.currencyMask(value);
    event.target.value = maskedValue;
  }

  @HostListener('blur', ['$event'])
  onBlur(event: any): void {
    const value = event.target.value;
    const maskedValue = this.maskService.currencyMask(value);
    event.target.value = maskedValue;
  }
}
