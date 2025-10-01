export interface ParcelaResumo {
  numero: number;
  vencimento: string;
  valor: number;
  diasEmAtraso: number;
  valorAtualizado: number;
}

export interface TituloResumo {
  numeroTitulo: string;
  nomeDevedor: string;
  cpfDevedor: string;
  jurosAoMes: number;
  multaPercentual: number;
  valorOriginal: number;
  valorAtualizado: number;
  parcelaQuantidade: number;
  parcelas: ParcelaResumo[];
}

export interface ParcelaEntrada {
  numero: number;
  vencimento: string;
  valor: number;
}

export interface TituloEntrada {
  numeroTitulo: string;
  nomeDevedor: string;
  cpfDevedor: string;
  jurosAoMes: number;
  multaPercentual: number;
  parcelas: ParcelaEntrada[];
}

