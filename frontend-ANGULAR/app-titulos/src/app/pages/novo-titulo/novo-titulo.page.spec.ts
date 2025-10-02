import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { NovoTituloPage } from './novo-titulo.page';
import { TituloService } from '../../shared/services/titulo.service';
import { MaskService } from '../../shared/services/mask.service';

describe('NovoTituloPage', () => {
  let component: NovoTituloPage;
  let fixture: ComponentFixture<NovoTituloPage>;
  let mockTituloService: jasmine.SpyObj<TituloService>;
  let mockMaskService: jasmine.SpyObj<MaskService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const tituloServiceSpy = jasmine.createSpyObj('TituloService', ['criar']);
    const maskServiceSpy = jasmine.createSpyObj('MaskService', ['removeMask', 'removePercentageMask', 'removeCurrencyMask']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const activatedRouteSpy = jasmine.createSpyObj('ActivatedRoute', [], {
      snapshot: { url: [] },
      url: of([])
    });

    await TestBed.configureTestingModule({
      imports: [NovoTituloPage, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: TituloService, useValue: tituloServiceSpy },
        { provide: MaskService, useValue: maskServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NovoTituloPage);
    component = fixture.componentInstance;
    mockTituloService = TestBed.inject(TituloService) as jasmine.SpyObj<TituloService>;
    mockMaskService = TestBed.inject(MaskService) as jasmine.SpyObj<MaskService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with default values', () => {
    expect(component.formulario.get('numeroTitulo')?.value).toBe('');
    expect(component.formulario.get('nomeDevedor')?.value).toBe('');
    expect(component.formulario.get('cpfDevedor')?.value).toBe('');
    expect(component.formulario.get('jurosAoMes')?.value).toBe(0);
    expect(component.formulario.get('multaPercentual')?.value).toBe(0);
    expect(component.parcelas.length).toBe(1);
  });

  it('should add new parcel when adicionarParcela is called', () => {
    const initialLength = component.parcelas.length;
    component.adicionarParcela();
    expect(component.parcelas.length).toBe(initialLength + 1);
  });

  it('should not remove parcel when only one exists', () => {
    component.removerParcela(0);
    expect(component.parcelas.length).toBe(1);
  });

  it('should remove parcel when more than one exists', () => {
    component.adicionarParcela();
    const initialLength = component.parcelas.length;
    component.removerParcela(0);
    expect(component.parcelas.length).toBe(initialLength - 1);
  });

  it('should mark form as touched and show error message when form is invalid', () => {
    spyOn(component.formulario, 'markAllAsTouched');

    const mockResponse = {
      numeroTitulo: '12345',
      nomeDevedor: 'Teste',
      cpfDevedor: '12345678901',
      jurosAoMes: 0.12,
      multaPercentual: 0.05,
      valorOriginal: 1000,
      valorAtualizado: 1050,
      parcelaQuantidade: 1,
      parcelas: []
    };
    mockTituloService.criar.and.returnValue(of(mockResponse));
    mockMaskService.removeMask.and.returnValue('');
    
    component.salvar();
    expect(component.formulario.markAllAsTouched).toHaveBeenCalled();
    expect(component.mensagem).toBe('Preencha todos os campos obrigatórios.');
  });

  it('should call service and navigate when form is valid', () => {

    const mockResponse = {
      numeroTitulo: '12345',
      nomeDevedor: 'Teste',
      cpfDevedor: '12345678901',
      jurosAoMes: 0.12,
      multaPercentual: 0.05,
      valorOriginal: 1000,
      valorAtualizado: 1050,
      parcelaQuantidade: 1,
      parcelas: []
    };

    mockTituloService.criar.and.returnValue(of(mockResponse));
    mockMaskService.removeMask.and.returnValue('12345678901');


    component.formulario.patchValue({
      numeroTitulo: '12345',
      nomeDevedor: 'Teste',
      cpfDevedor: '123.456.789-01',
      jurosAoMes: 12,
      multaPercentual: 5
    });
    component.parcelas.at(0).patchValue({
      numero: 1,
      vencimento: '2025-12-31',
      valor: 1000
    });


    component.salvar();

 
    expect(mockTituloService.criar).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/titulos']);
    expect(component.mensagem).toBe('Título cadastrado com sucesso!');
  });

  it('should handle service error', () => {
    // Arrange
    const mockError = { message: 'Erro de conexão' };
    mockTituloService.criar.and.returnValue(of(null).pipe(() => {
      throw mockError;
    }));
    mockMaskService.removeMask.and.returnValue('12345678901');


    component.formulario.patchValue({
      numeroTitulo: '12345',
      nomeDevedor: 'Teste',
      cpfDevedor: '123.456.789-01',
      jurosAoMes: 12,
      multaPercentual: 5
    });
    component.parcelas.at(0).patchValue({
      numero: 1,
      vencimento: '2025-12-31',
      valor: 1000
    });


    component.salvar();


    expect(component.mensagem).toContain('Erro ao cadastrar título');
  });

  it('should convert percentage to decimal correctly', () => {
    expect(component['converterPercentualParaDecimal'](12)).toBe(0.12);
    expect(component['converterPercentualParaDecimal'](5.5)).toBe(0.055);
  });

  it('should create parcel form group with default values', () => {
    const parcela = component['criarParcela']();
    expect(parcela.get('numero')?.value).toBe(1);
    expect(parcela.get('vencimento')?.value).toBe('');
    expect(parcela.get('valor')?.value).toBe(0.01);
  });

  it('should get form errors correctly', () => {
    component.formulario.get('numeroTitulo')?.setErrors({ required: true });
    const errors = component['getFormErrors']();
    expect(errors['numeroTitulo']).toEqual({ required: true });
  });
});
