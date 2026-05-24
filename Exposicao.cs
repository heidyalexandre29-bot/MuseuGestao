using System;
using System.Collections.Generic;
using System.Linq;

namespace MuseuGestao
{
    public class Exposicao : IExibivel
    {
        private static int _contadorId = 1;

        public int Id { get; }
        public string Nome { get; private set; }
        public string Curador { get; private set; }
        public TipoExposicao Tipo { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime? DataFim { get; private set; }
        public bool Encerrada { get; private set; }

        private readonly List<Peca> _pecas;
        public IReadOnlyList<Peca> Pecas => _pecas.AsReadOnly();

        public Exposicao(string nome, string curador, TipoExposicao tipo, DateTime dataInicio, DateTime? dataFim = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da exposição não pode estar vazio.");
            if (string.IsNullOrWhiteSpace(curador))
                throw new ArgumentException("O curador não pode estar vazio.");

            Id = _contadorId++;
            Nome = nome;
            Curador = curador;
            Tipo = tipo;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Encerrada = false;
            _pecas = new List<Peca>();
        }

        public void AdicionarPeca(Peca peca)
        {
            if (Encerrada)
                throw new ExposicaoEncerradaException(Nome);

            if (!peca.EstaDisponivel())
                throw new PecaNaoDisponivelException(peca.Nome);

            if (_pecas.Any(p => p.Id == peca.Id))
                throw new InvalidOperationException($"A peça '{peca.Nome}' já está nesta exposição.");

            _pecas.Add(peca);
        }

        public void RemoverPeca(Peca peca)
        {
            if (Encerrada)
                throw new ExposicaoEncerradaException(Nome);

            bool removida = _pecas.Remove(peca);
            if (!removida)
                throw new InvalidOperationException($"A peça '{peca.Nome}' não pertence a esta exposição.");
        }

        public void Encerrar()
        {
            if (Encerrada)
                throw new InvalidOperationException($"A exposição '{Nome}' já se encontra encerrada.");

            Encerrada = true;
            DataFim = DateTime.Now;
        }

        public string ObterDescricaoExibicao()
        {
            string estado = Encerrada ? "ENCERRADA" : "ACTIVA";
            string periodo = DataFim.HasValue
                ? $"{DataInicio:dd/MM/yyyy} → {DataFim.Value:dd/MM/yyyy}"
                : $"desde {DataInicio:dd/MM/yyyy}";
            return $"[Exposição #{Id}] {Nome} | {Tipo} | Curador: {Curador} | {periodo} | {estado} | {_pecas.Count} peça(s)";
        }

        public bool EstaDisponivel()
        {
            return !Encerrada;
        }

        public override string ToString()
        {
            return ObterDescricaoExibicao();
        }
    }
}
