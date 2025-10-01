namespace Desafio.Api.Models;

public class ParcelaDivida
{
    public int Numero { get; set; }
    public string Vencimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class TituloDivida
{
    public string NumeroTitulo { get; set; } = string.Empty;
    public string NomeDevedor { get; set; } = string.Empty;
    public string CpfDevedor { get; set; } = string.Empty;
    public decimal JurosAoMes { get; set; }
    public decimal MultaPercentual { get; set; }
    public List<ParcelaDivida> Parcelas { get; set; } = new();

    public decimal ValorOriginal => Parcelas.Sum(p => p.Valor);
}

