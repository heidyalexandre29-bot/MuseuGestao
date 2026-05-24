using System;

namespace MuseuGestao
{
    public class PecaNaoDisponivelException : Exception
    {
        public string NomePeca { get; }

        public PecaNaoDisponivelException(string nomePeca)
            : base($"A peça '{nomePeca}' não está disponível.")
        {
            NomePeca = nomePeca;
        }

        public PecaNaoDisponivelException(string nomePeca, string motivo)
            : base($"A peça '{nomePeca}' não está disponível: {motivo}")
        {
            NomePeca = nomePeca;
        }
    }

    public class ExposicaoEncerradaException : Exception
    {
        public string NomeExposicao { get; }

        public ExposicaoEncerradaException(string nomeExposicao)
            : base($"A exposição '{nomeExposicao}' encontra-se encerrada.")
        {
            NomeExposicao = nomeExposicao;
        }
    }

    public class CapacidadeExcedidaException : Exception
    {
        public int CapacidadeMaxima { get; }
        public int TentativaInscricao { get; }

        public CapacidadeExcedidaException(int capacidadeMaxima, int tentativaInscricao)
            : base($"Capacidade excedida. Máximo permitido: {capacidadeMaxima} visitantes. Tentativa: {tentativaInscricao}.")
        {
            CapacidadeMaxima = capacidadeMaxima;
            TentativaInscricao = tentativaInscricao;
        }
    }
}
