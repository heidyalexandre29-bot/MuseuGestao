using System;
using System.Collections.Generic;
using System.Linq;

namespace MuseuGestao
{
    public class GestorMuseu
    {
        private readonly List<Peca> _pecas;
        private readonly List<Exposicao> _exposicoes;
        private readonly List<Visitante> _visitantes;
        private readonly List<Bilhete> _bilhetes;
        private readonly List<VisitaGuiada> _visitasGuiadas;

        public IReadOnlyList<Peca> Pecas => _pecas.AsReadOnly();
        public IReadOnlyList<Exposicao> Exposicoes => _exposicoes.AsReadOnly();
        public IReadOnlyList<Visitante> Visitantes => _visitantes.AsReadOnly();
        public IReadOnlyList<Bilhete> Bilhetes => _bilhetes.AsReadOnly();
        public IReadOnlyList<VisitaGuiada> VisitasGuiadas => _visitasGuiadas.AsReadOnly();

        public GestorMuseu()
        {
            _pecas = new List<Peca>();
            _exposicoes = new List<Exposicao>();
            _visitantes = new List<Visitante>();
            _bilhetes = new List<Bilhete>();
            _visitasGuiadas = new List<VisitaGuiada>();
        }

        public Peca CatalogarPeca(string nome, string descricao, string periodoHistorico, string origem, EstadoConservacao estado)
        {
            var peca = new Peca(nome, descricao, periodoHistorico, origem, estado);
            _pecas.Add(peca);
            return peca;
        }

        public Peca ObterPecaPorId(int id)
        {
            return _pecas.FirstOrDefault(p => p.Id == id);
        }

        public void IniciarEmprestimo(int pecaId, string museuDestino, DateTime dataPrevistaRegresso)
        {
            var peca = ObterPecaPorId(pecaId);
            if (peca == null) throw new InvalidOperationException($"Peça com ID {pecaId} não encontrada.");
            peca.IniciarEmprestimo(museuDestino, dataPrevistaRegresso);
        }

        public void DevolverEmprestimo(int pecaId)
        {
            var peca = ObterPecaPorId(pecaId);
            if (peca == null) throw new InvalidOperationException($"Peça com ID {pecaId} não encontrada.");
            peca.DevolverEmprestimo();
        }

        public Exposicao CriarExposicao(string nome, string curador, TipoExposicao tipo, DateTime dataInicio, DateTime? dataFim = null)
        {
            var exposicao = new Exposicao(nome, curador, tipo, dataInicio, dataFim);
            _exposicoes.Add(exposicao);
            return exposicao;
        }

        public Exposicao ObterExposicaoPorId(int id)
        {
            return _exposicoes.FirstOrDefault(e => e.Id == id);
        }

        public void AdicionarPecaAExposicao(int exposicaoId, int pecaId)
        {
            var exposicao = ObterExposicaoPorId(exposicaoId);
            if (exposicao == null) throw new InvalidOperationException($"Exposição com ID {exposicaoId} não encontrada.");
            var peca = ObterPecaPorId(pecaId);
            if (peca == null) throw new InvalidOperationException($"Peça com ID {pecaId} não encontrada.");
            exposicao.AdicionarPeca(peca);
        }

        public void EncerrarExposicao(int exposicaoId)
        {
            var exposicao = ObterExposicaoPorId(exposicaoId);
            if (exposicao == null) throw new InvalidOperationException($"Exposição com ID {exposicaoId} não encontrada.");
            exposicao.Encerrar();
        }

        public Visitante RegistarVisitante(string nome, string contacto, CategoriaBilhete categoria)
        {
            var visitante = new Visitante(nome, contacto, categoria);
            _visitantes.Add(visitante);
            return visitante;
        }

        public Visitante ObterVisitantePorId(int id)
        {
            return _visitantes.FirstOrDefault(v => v.Id == id);
        }

        public Bilhete VenderBilhete(int visitanteId, int exposicaoId)
        {
            var visitante = ObterVisitantePorId(visitanteId);
            if (visitante == null) throw new InvalidOperationException($"Visitante com ID {visitanteId} não encontrado.");
            var exposicao = ObterExposicaoPorId(exposicaoId);
            if (exposicao == null) throw new InvalidOperationException($"Exposição com ID {exposicaoId} não encontrada.");

            var bilhete = new Bilhete(visitante, exposicao);
            _bilhetes.Add(bilhete);
            return bilhete;
        }

        public VisitaGuiada RegistarVisitaGuiada(string guia, int exposicaoId, DateTime dataHora, int capacidade)
        {
            var exposicao = ObterExposicaoPorId(exposicaoId);
            if (exposicao == null) throw new InvalidOperationException($"Exposição com ID {exposicaoId} não encontrada.");

            var visita = new VisitaGuiada(guia, exposicao, dataHora, capacidade);
            _visitasGuiadas.Add(visita);
            return visita;
        }

        public void InscreverVisitanteEmVisita(int visitaId, int visitanteId)
        {
            var visita = _visitasGuiadas.FirstOrDefault(v => v.Id == visitaId);
            if (visita == null) throw new InvalidOperationException($"Visita guiada com ID {visitaId} não encontrada.");
            var visitante = ObterVisitantePorId(visitanteId);
            if (visitante == null) throw new InvalidOperationException($"Visitante com ID {visitanteId} não encontrado.");
            visita.InscreverVisitante(visitante);
        }

        public void GerarRelatorio(DateTime? dataInicio = null, DateTime? dataFim = null, int? exposicaoId = null)
        {
            IEnumerable<Bilhete> bilhetesFiltrados = _bilhetes;

            if (dataInicio.HasValue)
                bilhetesFiltrados = bilhetesFiltrados.Where(b => b.DataEmissao >= dataInicio.Value);
            if (dataFim.HasValue)
                bilhetesFiltrados = bilhetesFiltrados.Where(b => b.DataEmissao <= dataFim.Value.AddDays(1).AddSeconds(-1));
            if (exposicaoId.HasValue)
                bilhetesFiltrados = bilhetesFiltrados.Where(b => b.Exposicao.Id == exposicaoId.Value);

            var lista = bilhetesFiltrados.ToList();

            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          RELATÓRIO DE VISITAÇÃO                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

            string periodoStr;
            if (dataInicio.HasValue && dataFim.HasValue)
                periodoStr = $"  Período : {dataInicio.Value:dd/MM/yyyy} → {dataFim.Value:dd/MM/yyyy}";
            else if (dataInicio.HasValue)
                periodoStr = $"  Período : a partir de {dataInicio.Value:dd/MM/yyyy}";
            else if (dataFim.HasValue)
                periodoStr = $"  Período : até {dataFim.Value:dd/MM/yyyy}";
            else
                periodoStr = "  Período : todos os registos";

            Console.WriteLine(periodoStr);

            if (exposicaoId.HasValue)
            {
                var expo = ObterExposicaoPorId(exposicaoId.Value);
                Console.WriteLine($"  Exposição: {expo?.Nome ?? "Desconhecida"}");
            }
            else
            {
                Console.WriteLine("  Exposição: todas");
            }

            Console.WriteLine($"  Total de bilhetes : {lista.Count}");
            Console.WriteLine($"  Receita total     : {lista.Sum(b => b.Preco):C}");
            Console.WriteLine();

            var porExposicao = lista.GroupBy(b => b.Exposicao.Nome).OrderByDescending(g => g.Count());
            foreach (var grupo in porExposicao)
            {
                Console.WriteLine($"  ▸ {grupo.Key}");
                Console.WriteLine($"      Bilhetes  : {grupo.Count()}");
                Console.WriteLine($"      Receita   : {grupo.Sum(b => b.Preco):C}");
                Console.WriteLine($"      Adultos   : {grupo.Count(b => b.Categoria == CategoriaBilhete.Adulto)}");
                Console.WriteLine($"      Crianças  : {grupo.Count(b => b.Categoria == CategoriaBilhete.Crianca)}");
                Console.WriteLine($"      Séniors   : {grupo.Count(b => b.Categoria == CategoriaBilhete.Senior)}");
                Console.WriteLine();
            }

            if (!lista.Any())
                Console.WriteLine("  Sem registos para os filtros seleccionados.");

            Console.WriteLine("══════════════════════════════════════════════════════════════");
        }

        public List<IExibivel> ObterTodosExibiveis()
        {
            var lista = new List<IExibivel>();
            lista.AddRange(_pecas.Cast<IExibivel>());
            lista.AddRange(_exposicoes.Cast<IExibivel>());
            return lista;
        }
    }
}
