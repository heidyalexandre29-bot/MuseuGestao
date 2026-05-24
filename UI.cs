using System;
using System.Linq;

namespace MuseuGestao
{
    public class UI
    {
        private readonly GestorMuseu _gestor;

        public UI(GestorMuseu gestor)
        {
            _gestor = gestor;
        }

        public void Iniciar()
        {
            Console.Clear();
            MostrarCabecalho();
            bool sair = false;

            while (!sair)
            {
                MostrarMenuPrincipal();
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": MenuPecas(); break;
                    case "2": MenuExposicoes(); break;
                    case "3": MenuVisitantes(); break;
                    case "4": MenuBilhetes(); break;
                    case "5": MenuVisitasGuiadas(); break;
                    case "6": MenuEmprestimos(); break;
                    case "7": MenuRelatorio(); break;
                    case "8": MenuExibiveis(); break;
                    case "0": sair = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("  Sistema encerrado. Até breve.");
            Console.WriteLine();
        }

        private void MostrarMenuPrincipal()
        {
            Console.WriteLine();
            Console.WriteLine("  ┌─────────────────────────────────────────┐");
            Console.WriteLine("  │         SISTEMA DE GESTÃO DE MUSEU      │");
            Console.WriteLine("  ├─────────────────────────────────────────┤");
            Console.WriteLine("  │  1. Gerir Peças                         │");
            Console.WriteLine("  │  2. Gerir Exposições                    │");
            Console.WriteLine("  │  3. Gerir Visitantes                    │");
            Console.WriteLine("  │  4. Venda de Bilhetes                   │");
            Console.WriteLine("  │  5. Visitas Guiadas                     │");
            Console.WriteLine("  │  6. Empréstimos de Peças                │");
            Console.WriteLine("  │  7. Relatório de Visitação              │");
            Console.WriteLine("  │  8. Listar Tudo (IExibivel)             │");
            Console.WriteLine("  │  0. Sair                                │");
            Console.WriteLine("  └─────────────────────────────────────────┘");
            Console.Write("  Opção: ");
        }

        private void MenuPecas()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── PEÇAS ──────────────────────────────────");
                Console.WriteLine("  1. Catalogar nova peça");
                Console.WriteLine("  2. Listar todas as peças");
                Console.WriteLine("  3. Actualizar estado de conservação");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": CatalogarPeca(); break;
                    case "2": ListarPecas(); break;
                    case "3": ActualizarEstadoPeca(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void CatalogarPeca()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Catalogar nova peça");
            string nome = LerTexto("  Nome da peça");
            string descricao = LerTexto("  Descrição");
            string periodo = LerTexto("  Período histórico");
            string origem = LerTexto("  Origem");
            EstadoConservacao estado = LerEnum<EstadoConservacao>("  Estado de conservação");

            try
            {
                var peca = _gestor.CatalogarPeca(nome, descricao, periodo, origem, estado);
                MostrarSucesso($"Peça catalogada com sucesso. ID: {peca.Id}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void ListarPecas()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Peças do acervo");
            if (!_gestor.Pecas.Any())
            {
                Console.WriteLine("  (sem peças registadas)");
                return;
            }
            foreach (var p in _gestor.Pecas)
                Console.WriteLine($"  {p}");
        }

        private void ActualizarEstadoPeca()
        {
            ListarPecas();
            if (!_gestor.Pecas.Any()) return;
            int id = LerInteiro("  ID da peça");
            var peca = _gestor.ObterPecaPorId(id);
            if (peca == null) { MostrarErro("Peça não encontrada."); return; }
            EstadoConservacao estado = LerEnum<EstadoConservacao>("  Novo estado");
            peca.ActualizarEstado(estado);
            MostrarSucesso("Estado actualizado.");
        }

        private void MenuExposicoes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── EXPOSIÇÕES ─────────────────────────────");
                Console.WriteLine("  1. Criar nova exposição");
                Console.WriteLine("  2. Listar exposições");
                Console.WriteLine("  3. Adicionar peça a exposição");
                Console.WriteLine("  4. Encerrar exposição");
                Console.WriteLine("  5. Ver peças de uma exposição");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": CriarExposicao(); break;
                    case "2": ListarExposicoes(); break;
                    case "3": AdicionarPecaExposicao(); break;
                    case "4": EncerrarExposicao(); break;
                    case "5": VerPecasExposicao(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void CriarExposicao()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Criar nova exposição");
            string nome = LerTexto("  Nome");
            string curador = LerTexto("  Curador");
            TipoExposicao tipo = LerEnum<TipoExposicao>("  Tipo");

            try
            {
                var expo = _gestor.CriarExposicao(nome, curador, tipo, DateTime.Now);
                MostrarSucesso($"Exposição criada com sucesso. ID: {expo.Id}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void ListarExposicoes()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Exposições");
            if (!_gestor.Exposicoes.Any())
            {
                Console.WriteLine("  (sem exposições)");
                return;
            }
            foreach (var e in _gestor.Exposicoes)
                Console.WriteLine($"  {e}");
        }

        private void AdicionarPecaExposicao()
        {
            ListarExposicoes();
            if (!_gestor.Exposicoes.Any()) return;
            int expId = LerInteiro("  ID da exposição");
            ListarPecas();
            if (!_gestor.Pecas.Any()) return;
            int pecaId = LerInteiro("  ID da peça");

            try
            {
                _gestor.AdicionarPecaAExposicao(expId, pecaId);
                MostrarSucesso("Peça adicionada à exposição.");
            }
            catch (PecaNaoDisponivelException ex)
            {
                MostrarErro($"[PecaNaoDisponivelException] {ex.Message}");
            }
            catch (ExposicaoEncerradaException ex)
            {
                MostrarErro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void EncerrarExposicao()
        {
            ListarExposicoes();
            if (!_gestor.Exposicoes.Any()) return;
            int id = LerInteiro("  ID da exposição a encerrar");

            try
            {
                _gestor.EncerrarExposicao(id);
                MostrarSucesso("Exposição encerrada.");
            }
            catch (ExposicaoEncerradaException ex)
            {
                MostrarErro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void VerPecasExposicao()
        {
            ListarExposicoes();
            if (!_gestor.Exposicoes.Any()) return;
            int id = LerInteiro("  ID da exposição");
            var expo = _gestor.ObterExposicaoPorId(id);
            if (expo == null) { MostrarErro("Exposição não encontrada."); return; }

            Console.WriteLine();
            Console.WriteLine($"  Peças da exposição '{expo.Nome}':");
            if (!expo.Pecas.Any())
                Console.WriteLine("  (sem peças associadas)");
            else
                foreach (var p in expo.Pecas)
                    Console.WriteLine($"  {p}");
        }

        private void MenuVisitantes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── VISITANTES ─────────────────────────────");
                Console.WriteLine("  1. Registar visitante");
                Console.WriteLine("  2. Listar visitantes");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": RegistarVisitante(); break;
                    case "2": ListarVisitantes(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void RegistarVisitante()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Registar visitante");
            string nome = LerTexto("  Nome");
            string contacto = LerTexto("  Contacto (email ou telefone)");
            CategoriaBilhete categoria = LerEnum<CategoriaBilhete>("  Categoria");

            try
            {
                var v = _gestor.RegistarVisitante(nome, contacto, categoria);
                MostrarSucesso($"Visitante registado. ID: {v.Id}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void ListarVisitantes()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Visitantes");
            if (!_gestor.Visitantes.Any())
            {
                Console.WriteLine("  (sem visitantes)");
                return;
            }
            foreach (var v in _gestor.Visitantes)
                Console.WriteLine($"  {v}");
        }

        private void MenuBilhetes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── BILHETES ───────────────────────────────");
                Console.WriteLine("  1. Vender bilhete");
                Console.WriteLine("  2. Listar bilhetes emitidos");
                Console.WriteLine("  3. Consultar tabela de preços");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": VenderBilhete(); break;
                    case "2": ListarBilhetes(); break;
                    case "3": MostrarTabelaPrecos(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void VenderBilhete()
        {
            ListarVisitantes();
            if (!_gestor.Visitantes.Any()) return;
            int vidId = LerInteiro("  ID do visitante");

            ListarExposicoes();
            if (!_gestor.Exposicoes.Any()) return;
            int expId = LerInteiro("  ID da exposição");

            try
            {
                var bilhete = _gestor.VenderBilhete(vidId, expId);
                MostrarSucesso($"Bilhete emitido: {bilhete}");
            }
            catch (ExposicaoEncerradaException ex)
            {
                MostrarErro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void ListarBilhetes()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Bilhetes emitidos");
            if (!_gestor.Bilhetes.Any())
            {
                Console.WriteLine("  (sem bilhetes)");
                return;
            }
            foreach (var b in _gestor.Bilhetes)
                Console.WriteLine($"  {b}");
        }

        private void MostrarTabelaPrecos()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Tabela de preços");
            Console.WriteLine($"  Adulto  : {Bilhete.ObterPreco(CategoriaBilhete.Adulto):C}");
            Console.WriteLine($"  Criança : {Bilhete.ObterPreco(CategoriaBilhete.Crianca):C}");
            Console.WriteLine($"  Sénior  : {Bilhete.ObterPreco(CategoriaBilhete.Senior):C}");
        }

        private void MenuVisitasGuiadas()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── VISITAS GUIADAS ────────────────────────");
                Console.WriteLine("  1. Registar visita guiada");
                Console.WriteLine("  2. Inscrever visitante numa visita");
                Console.WriteLine("  3. Listar visitas guiadas");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": RegistarVisitaGuiada(); break;
                    case "2": InscreverVisitante(); break;
                    case "3": ListarVisitasGuiadas(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void RegistarVisitaGuiada()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Registar visita guiada");
            string guia = LerTexto("  Nome do guia responsável");
            ListarExposicoes();
            if (!_gestor.Exposicoes.Any()) return;
            int expId = LerInteiro("  ID da exposição");
            int capacidade = LerInteiro("  Capacidade máxima do grupo");

            try
            {
                var visita = _gestor.RegistarVisitaGuiada(guia, expId, DateTime.Now.AddDays(1), capacidade);
                MostrarSucesso($"Visita guiada registada. ID: {visita.Id}");
            }
            catch (ExposicaoEncerradaException ex)
            {
                MostrarErro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void InscreverVisitante()
        {
            ListarVisitasGuiadas();
            if (!_gestor.VisitasGuiadas.Any()) return;
            int visitaId = LerInteiro("  ID da visita");
            ListarVisitantes();
            if (!_gestor.Visitantes.Any()) return;
            int visitanteId = LerInteiro("  ID do visitante");

            try
            {
                _gestor.InscreverVisitanteEmVisita(visitaId, visitanteId);
                MostrarSucesso("Visitante inscrito na visita guiada.");
            }
            catch (CapacidadeExcedidaException ex)
            {
                MostrarErro($"[CapacidadeExcedidaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void ListarVisitasGuiadas()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Visitas guiadas");
            if (!_gestor.VisitasGuiadas.Any())
            {
                Console.WriteLine("  (sem visitas registadas)");
                return;
            }
            foreach (var v in _gestor.VisitasGuiadas)
                Console.WriteLine($"  {v}");
        }

        private void MenuEmprestimos()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ── EMPRÉSTIMOS ────────────────────────────");
                Console.WriteLine("  1. Iniciar empréstimo de peça");
                Console.WriteLine("  2. Registar devolução");
                Console.WriteLine("  3. Ver peças em empréstimo");
                Console.WriteLine("  0. Voltar");
                Console.Write("  Opção: ");
                string opcao = LerOpcao();

                switch (opcao)
                {
                    case "1": IniciarEmprestimo(); break;
                    case "2": DevolverEmprestimo(); break;
                    case "3": VerEmprestimos(); break;
                    case "0": voltar = true; break;
                    default: MostrarErro("Opção inválida."); break;
                }
            }
        }

        private void IniciarEmprestimo()
        {
            ListarPecas();
            if (!_gestor.Pecas.Any()) return;
            int id = LerInteiro("  ID da peça a emprestar");
            string museu = LerTexto("  Museu de destino");
            Console.Write("  Data prevista de regresso (dd/MM/yyyy): ");
            string dataStr = Console.ReadLine() ?? "";
            if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataRegresso))
            {
                MostrarErro("Formato de data inválido.");
                return;
            }

            try
            {
                _gestor.IniciarEmprestimo(id, museu, dataRegresso);
                MostrarSucesso("Empréstimo iniciado.");
            }
            catch (PecaNaoDisponivelException ex)
            {
                MostrarErro($"[PecaNaoDisponivelException] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void DevolverEmprestimo()
        {
            VerEmprestimos();
            int id = LerInteiro("  ID da peça a devolver");

            try
            {
                _gestor.DevolverEmprestimo(id);
                MostrarSucesso("Devolução registada.");
            }
            catch (Exception ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void VerEmprestimos()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Peças em empréstimo");
            var emprestadas = _gestor.Pecas.Where(p => p.EmEmprestimo).ToList();
            if (!emprestadas.Any())
            {
                Console.WriteLine("  (nenhuma peça em empréstimo)");
                return;
            }
            foreach (var p in emprestadas)
                Console.WriteLine($"  {p}");
        }

        private void MenuRelatorio()
        {
            Console.WriteLine();
            Console.WriteLine("  ── RELATÓRIO DE VISITAÇÃO ─────────────────");
            Console.WriteLine("  1. Relatório geral (todos os períodos)");
            Console.WriteLine("  2. Relatório por período");
            Console.WriteLine("  3. Relatório por exposição");
            Console.WriteLine("  4. Relatório por período e exposição");
            Console.Write("  Opção: ");
            string opcao = LerOpcao();

            DateTime? inicio = null, fim = null;
            int? expId = null;

            if (opcao == "2" || opcao == "4")
            {
                Console.Write("  Data início (dd/MM/yyyy): ");
                string di = Console.ReadLine() ?? "";
                if (DateTime.TryParseExact(di, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dti))
                    inicio = dti;

                Console.Write("  Data fim (dd/MM/yyyy): ");
                string df = Console.ReadLine() ?? "";
                if (DateTime.TryParseExact(df, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dtf))
                    fim = dtf;
            }

            if (opcao == "3" || opcao == "4")
            {
                ListarExposicoes();
                expId = LerInteiro("  ID da exposição");
            }

            _gestor.GerarRelatorio(inicio, fim, expId);
        }

        private void MenuExibiveis()
        {
            Console.WriteLine();
            Console.WriteLine("  >> Todos os itens do sistema (via IExibivel)");
            var itens = _gestor.ObterTodosExibiveis();
            if (!itens.Any())
            {
                Console.WriteLine("  (sem itens)");
                return;
            }
            foreach (var item in itens)
            {
                string disponivel = item.EstaDisponivel() ? "DISPONÍVEL" : "INDISPONÍVEL";
                Console.WriteLine($"  [{disponivel}] {item.ObterDescricaoExibicao()}");
            }
        }

        private static string LerOpcao()
        {
            return (Console.ReadLine() ?? "").Trim();
        }

        private static string LerTexto(string prompt)
        {
            string val;
            do
            {
                Console.Write($"{prompt}: ");
                val = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(val))
                    MostrarErro("Campo obrigatório.");
            } while (string.IsNullOrWhiteSpace(val));
            return val;
        }

        private static int LerInteiro(string prompt)
        {
            int val;
            while (true)
            {
                Console.Write($"{prompt}: ");
                string entrada = Console.ReadLine() ?? "";
                if (int.TryParse(entrada, out val)) break;
                MostrarErro("Introduza um número inteiro válido.");
            }
            return val;
        }

        private static T LerEnum<T>(string prompt) where T : struct, Enum
        {
            var valores = (T[])Enum.GetValues(typeof(T));
            Console.WriteLine($"{prompt}:");
            for (int i = 0; i < valores.Length; i++)
                Console.WriteLine($"    {i + 1}. {valores[i]}");

            while (true)
            {
                Console.Write("  Escolha: ");
                string entrada = Console.ReadLine() ?? "";
                if (int.TryParse(entrada, out int idx) && idx >= 1 && idx <= valores.Length)
                    return valores[idx - 1];
                MostrarErro("Opção inválida.");
            }
        }

        private static void MostrarErro(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {msg}");
            Console.ResetColor();
        }

        private static void MostrarSucesso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {msg}");
            Console.ResetColor();
        }

        private static void MostrarCabecalho()
        {
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║      MUSEU NACIONAL - GESTÃO INTERNA     ║");
            Console.WriteLine("  ║         POO II com C# - Projecto 23      ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
        }
    }
}
