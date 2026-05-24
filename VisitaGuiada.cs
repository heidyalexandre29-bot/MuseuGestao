using System;
using System.Collections.Generic;
using System.Linq;

namespace MuseuGestao
{
    public class VisitaGuiada
    {
        private static int _contadorId = 1;

        public int Id { get; }
        public string GuiaResponsavel { get; private set; }
        public Exposicao Exposicao { get; }
        public DateTime DataHora { get; private set; }
        public int CapacidadeMaxima { get; private set; }

        private readonly List<Visitante> _grupo;
        public IReadOnlyList<Visitante> Grupo => _grupo.AsReadOnly();
        public int NumeroParticipantes => _grupo.Count;

        public VisitaGuiada(string guiaResponsavel, Exposicao exposicao, DateTime dataHora, int capacidadeMaxima)
        {
            if (string.IsNullOrWhiteSpace(guiaResponsavel))
                throw new ArgumentException("O nome do guia não pode estar vazio.");
            if (exposicao.Encerrada)
                throw new ExposicaoEncerradaException(exposicao.Nome);
            if (capacidadeMaxima <= 0)
                throw new ArgumentException("A capacidade máxima tem de ser superior a zero.");

            Id = _contadorId++;
            GuiaResponsavel = guiaResponsavel;
            Exposicao = exposicao;
            DataHora = dataHora;
            CapacidadeMaxima = capacidadeMaxima;
            _grupo = new List<Visitante>();
        }

        public void InscreverVisitante(Visitante visitante)
        {
            if (_grupo.Count >= CapacidadeMaxima)
                throw new CapacidadeExcedidaException(CapacidadeMaxima, _grupo.Count + 1);

            if (_grupo.Any(v => v.Id == visitante.Id))
                throw new InvalidOperationException($"O visitante '{visitante.Nome}' já está inscrito nesta visita.");

            _grupo.Add(visitante);
        }

        public void RemoverVisitante(Visitante visitante)
        {
            bool removido = _grupo.Remove(visitante);
            if (!removido)
                throw new InvalidOperationException($"O visitante '{visitante.Nome}' não está inscrito nesta visita.");
        }

        public override string ToString()
        {
            return $"[Visita #{Id}] Guia: {GuiaResponsavel} | Expo: {Exposicao.Nome} | {DataHora:dd/MM/yyyy HH:mm} | {_grupo.Count}/{CapacidadeMaxima} inscritos";
        }
    }
}
