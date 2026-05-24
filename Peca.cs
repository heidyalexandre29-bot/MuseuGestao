using System;

namespace MuseuGestao
{
    public class Peca : IExibivel
    {
        private static int _contadorId = 1;

        public int Id { get; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public string PeriodoHistorico { get; private set; }
        public string Origem { get; private set; }
        public EstadoConservacao Estado { get; private set; }

        private bool _emEmprestimo;
        private string _museuDestino;
        private DateTime? _dataInicioEmprestimo;
        private DateTime? _dataPrevistaRegresso;

        public bool EmEmprestimo => _emEmprestimo;
        public string MuseuDestino => _museuDestino;

        public Peca(string nome, string descricao, string periodoHistorico, string origem, EstadoConservacao estado)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da peça não pode estar vazio.");
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição da peça não pode estar vazia.");
            if (string.IsNullOrWhiteSpace(periodoHistorico))
                throw new ArgumentException("O período histórico não pode estar vazio.");
            if (string.IsNullOrWhiteSpace(origem))
                throw new ArgumentException("A origem não pode estar vazia.");

            Id = _contadorId++;
            Nome = nome;
            Descricao = descricao;
            PeriodoHistorico = periodoHistorico;
            Origem = origem;
            Estado = estado;
            _emEmprestimo = false;
        }

        public void IniciarEmprestimo(string museuDestino, DateTime dataPrevistaRegresso)
        {
            if (!EstaDisponivel())
                throw new PecaNaoDisponivelException(Nome, _emEmprestimo
                    ? $"já emprestada ao museu '{_museuDestino}'"
                    : "encontra-se em restauro");

            _emEmprestimo = true;
            _museuDestino = museuDestino;
            _dataInicioEmprestimo = DateTime.Now;
            _dataPrevistaRegresso = dataPrevistaRegresso;
        }

        public void DevolverEmprestimo()
        {
            if (!_emEmprestimo)
                throw new InvalidOperationException($"A peça '{Nome}' não está em empréstimo.");

            _emEmprestimo = false;
            _museuDestino = null;
            _dataInicioEmprestimo = null;
            _dataPrevistaRegresso = null;
        }

        public void ActualizarEstado(EstadoConservacao novoEstado)
        {
            Estado = novoEstado;
        }

        public string ObterDescricaoExibicao()
        {
            string estadoEmprestimo = _emEmprestimo
                ? $" | Em empréstimo → {_museuDestino} (regresso: {_dataPrevistaRegresso:dd/MM/yyyy})"
                : "";
            return $"[Peça #{Id}] {Nome} | {PeriodoHistorico} | Origem: {Origem} | Estado: {Estado}{estadoEmprestimo}";
        }

        public bool EstaDisponivel()
        {
            return !_emEmprestimo && Estado != EstadoConservacao.Restauro;
        }

        public override string ToString()
        {
            return ObterDescricaoExibicao();
        }
    }
}
