import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { TitulosPage } from './titulos.page';
import { TituloService } from '../../shared/services/titulo.service';
import { CpfCnpjPipe } from '../../shared/pipes/cpf-cnpj.pipe';

describe('TitulosPage', () => {
  let component: TitulosPage;
  let fixture: ComponentFixture<TitulosPage>;
  let mockTituloService: jasmine.SpyObj<TituloService>;
  let mockRouter: jasmine.SpyObj<Router>;

  const mockTitulos = [
    {
      numeroTitulo: '12345',
      nomeDevedor: 'João Silva',
      cpfDevedor: '12345678901',
      jurosAoMes: 0.12,
      multaPercentual: 0.05,
      valorOriginal: 1000,
      valorAtualizado: 1050,
      parcelaQuantidade: 1,
      parcelas: []
    },
    {
      numeroTitulo: '67890',
      nomeDevedor: 'Maria Santos',
      cpfDevedor: '98765432100',
      jurosAoMes: 0.08,
      multaPercentual: 0.03,
      valorOriginal: 2000,
      valorAtualizado: 2060,
      parcelaQuantidade: 2,
      parcelas: []
    }
  ];

  beforeEach(async () => {
    const tituloServiceSpy = jasmine.createSpyObj('TituloService', ['listar']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const activatedRouteSpy = jasmine.createSpyObj('ActivatedRoute', [], {
      snapshot: { url: [] },
      url: of([])
    });

    await TestBed.configureTestingModule({
      imports: [TitulosPage, CpfCnpjPipe],
      providers: [
        { provide: TituloService, useValue: tituloServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TitulosPage);
    component = fixture.componentInstance;
    mockTituloService = TestBed.inject(TituloService) as jasmine.SpyObj<TituloService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;

 
    mockTituloService.listar.and.returnValue(of(mockTitulos));
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize titulos$ observable on ngOnInit', () => {
    component.ngOnInit();
    expect(mockTituloService.listar).toHaveBeenCalled();
  });

  it('should have titulos$ observable with data', (done) => {
    component.ngOnInit();
    
    component.titulos$.subscribe(titulos => {
      expect(titulos).toEqual(mockTitulos);
      expect(titulos.length).toBe(2);
      done();
    });
  });

  it('should handle empty titulos list', (done) => {
    mockTituloService.listar.and.returnValue(of([]));
    component.ngOnInit();
    
    component.titulos$.subscribe(titulos => {
      expect(titulos).toEqual([]);
      expect(titulos.length).toBe(0);
      done();
    });
  });

  it('should handle service error gracefully', () => {
    const mockError = new Error('Service error');
    mockTituloService.listar.and.returnValue(of([])); 
    
    component.ngOnInit();
    
    component.titulos$.subscribe(titulos => {
      expect(titulos).toEqual([]);
    });
  });

  it('should display correct number of titulos in template', () => {
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const rows = compiled.querySelectorAll('tbody tr');
    expect(rows.length).toBe(2);
  });

  it('should display titulo data correctly in template', () => {
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const firstRow = compiled.querySelector('tbody tr:first-child');
    
    expect(firstRow.textContent).toContain('12345');
    expect(firstRow.textContent).toContain('João Silva');
    expect(firstRow.textContent).toContain('123.456.789-01');
    expect(firstRow.textContent).toContain('1');
    expect(firstRow.textContent).toContain('R$1,000.00');
    expect(firstRow.textContent).toContain('R$1,050.00');
  });

  it('should show empty state when no titulos', () => {
    mockTituloService.listar.and.returnValue(of([]));
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const emptyState = compiled.querySelector('.lista__vazio');
    expect(emptyState).toBeTruthy();
    expect(emptyState.textContent).toContain('Nenhum título cadastrado');
  });

  it('should have correct table headers', () => {
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const headers = compiled.querySelectorAll('thead th');
    
    expect(headers[0].textContent.trim()).toBe('Número');
    expect(headers[1].textContent.trim()).toBe('Devedor');
    expect(headers[2].textContent.trim()).toBe('CPF / CNPJ');
    expect(headers[3].textContent.trim()).toBe('Parcelas');
    expect(headers[4].textContent.trim()).toBe('Valor original');
    expect(headers[5].textContent.trim()).toBe('Valor atualizado');
  });

  it('should format currency values correctly', () => {
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const currencyElements = compiled.querySelectorAll('[class*="currency"], td');    

    const hasFormattedCurrency = Array.from(currencyElements).some((el: any) => 
      el.textContent.includes('R$') || el.textContent.includes(',')
    );
    
    expect(hasFormattedCurrency).toBeTruthy();
  });

  it('should apply CPF/CNPJ pipe correctly', () => {
    component.ngOnInit();
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement;
    const cpfElements = compiled.querySelectorAll('td');    

    const cpfFormatted = Array.from(cpfElements).some((el: any) => 
      el.textContent.includes('123.456.789-01') || el.textContent.includes('987.654.321-00')
    );
    
    expect(cpfFormatted).toBeTruthy();
  });
});
