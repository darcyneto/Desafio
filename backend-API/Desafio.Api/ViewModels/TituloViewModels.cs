namespace Desafio.Api.ViewModels;

public class ParcelaEntrada
{
    public int Numero { get; set; }
    public string Vencimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class TituloEntrada
{
    public string NumeroTitulo { get; set; } = string.Empty;
    public string NomeDevedor { get; set; } = string.Empty;
    public string CpfDevedor { get; set; } = string.Empty;
    public decimal JurosAoMes { get; set; }
    public decimal MultaPercentual { get; set; }
    public List<ParcelaEntrada> Parcelas { get; set; } = new();
}

public class ParcelaResultado
{
    public int Numero { get; set; }
    public string Vencimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DiasEmAtraso { get; set; }
    public decimal ValorAtualizado { get; set; }
}

public class TituloResultado
{
    public string NumeroTitulo { get; set; } = string.Empty;
    public string NomeDevedor { get; set; } = string.Empty;
    public string CpfDevedor { get; set; } = string.Empty;
    public decimal JurosAoMes { get; set; }
    public decimal MultaPercentual { get; set; }
    public decimal ValorOriginal { get; set; }
    public decimal ValorAtualizado { get; set; }
    public int ParcelaQuantidade { get; set; }
    public List<ParcelaResultado> Parcelas { get; set; } = new();
}

