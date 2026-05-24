using System;
using System.Collections.Generic;

namespace MuseuGestao
{
    public class Bilhete
    {
        private static int _contadorId = 1;

        private static readonly Dictionary<CategoriaBilhete, decimal> TabelaPrecos = new Dictionary<CategoriaBilhete, decimal>
        {
            { CategoriaBilhete.Adulto,  10.00m },
            { CategoriaBilhete.Crianca,  5.00m },
            { CategoriaBilhete.Senior,   6.50m }
        };

        public int Id { get; }
        public Visitante Visitante { get; }
        public Exposicao Exposicao { get; }
        public CategoriaBilhete Categoria { get; }
        public decimal Preco { get; }
        public DateTime DataEmissao { get; }

        public Bilhete(Visitante visitante, Exposicao exposicao)
        {
            if (exposicao.Encerrada)
                throw new ExposicaoEncerradaException(exposicao.Nome);

            Id = _contadorId++;
            Visitante = visitante;
            Exposicao = exposicao;
            Categoria = visitante.Categoria;
            Preco = TabelaPrecos[Categoria];
            DataEmissao = DateTime.Now;
        }

        public static decimal ObterPreco(CategoriaBilhete categoria)
        {
            return TabelaPrecos[categoria];
        }

        public override string ToString()
        {
            return $"[Bilhete #{Id}] {Visitante.Nome} ({Categoria}) → {Exposicao.Nome} | {Preco:C} | Emitido: {DataEmissao:dd/MM/yyyy HH:mm}";
        }
    }
}
