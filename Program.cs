using System;
using System.Linq;

namespace MuseuGestao
{
    class Program
    {
        static GestorMuseu _gestor = new GestorMuseu();

        static void Main(string[] args)
        {
            Console.Clear();
            MostrarCabecalho();
            bool sair = false;

            while (!sair)
            {
                MostrarMenuPrincipal();
                string opcao = (Console.ReadLine() ?? "").Trim();

                switch (opcao)
                {
                    case "1": MenuTestes(); break;
                    case "2":
                        var ui = new UI(_gestor);
                        ui.Iniciar();
                        MostrarCabecalho();
                        break;
                    case "0": sair = true; break;
                    default:
                        Erro("Opção inválida.");
                        break;
                }
            }

            Console.WriteLine();
            Console.WriteLine("  Sistema encerrado. Até breve.");
            Console.WriteLine();
        }

        static void MenuTestes()
        {
            bool voltar = false;
            while (!voltar)
            {
                Console.WriteLine();
                Console.WriteLine("  ╔══════════════════════════════════════════╗");
                Console.WriteLine("  ║           TESTES DO SISTEMA              ║");
                Console.WriteLine("  ╠══════════════════════════════════════════╣");
                Console.WriteLine("  ║  1. Teste 1 — Catalogar peça             ║");
                Console.WriteLine("  ║  2. Teste 2 — Criar exposição            ║");
                Console.WriteLine("  ║  3. Teste 3 — Vender bilhete             ║");
                Console.WriteLine("  ║  4. Teste 4 — Registar visita guiada     ║");
                Console.WriteLine("  ║  5. Teste 5 — Controlar empréstimo       ║");
                Console.WriteLine("  ║  6. Teste 6 — Relatório de visitação     ║");
                Console.WriteLine("  ║  7. Executar todos os testes             ║");
                Console.WriteLine("  ║  0. Voltar                               ║");
                Console.WriteLine("  ╚══════════════════════════════════════════╝");
                Console.Write("  Opção: ");
                string op = (Console.ReadLine() ?? "").Trim();

                switch (op)
                {
                    case "1": Teste1_CatalogarPeca(); break;
                    case "2": Teste2_CriarExposicao(); break;
                    case "3": Teste3_VenderBilhete(); break;
                    case "4": Teste4_VisitaGuiada(); break;
                    case "5": Teste5_Emprestimo(); break;
                    case "6": Teste6_Relatorio(); break;
                    case "7": ExecutarTodosTestes(); break;
                    case "0": voltar = true; break;
                    default: Erro("Opção inválida."); break;
                }
            }
        }

        static void Teste1_CatalogarPeca()
        {
            Titulo("TESTE 1 — Catalogar Peça");
            Console.WriteLine("  Introduza os dados da peça a catalogar:");
            Console.WriteLine();

            string nome      = LerTexto("  Nome da peça");
            string descricao = LerTexto("  Descrição");
            string periodo   = LerTexto("  Período histórico");
            string origem    = LerTexto("  Origem");
            EstadoConservacao estado = LerEnum<EstadoConservacao>("  Estado de conservação");

            try
            {
                var peca = _gestor.CatalogarPeca(nome, descricao, periodo, origem, estado);
                Sucesso($"Peça catalogada com sucesso!");
                Console.WriteLine($"  {peca}");
            }
            catch (ArgumentException ex)
            {
                Erro($"Dados inválidos: {ex.Message}");
            }

            Pausa();
        }

        static void Teste2_CriarExposicao()
        {
            Titulo("TESTE 2 — Criar Exposição");

            if (!_gestor.Pecas.Any())
            {
                Console.WriteLine("  Não existem peças. Execute primeiro o Teste 1.");
                Pausa();
                return;
            }

            Console.WriteLine("  Introduza os dados da exposição:");
            Console.WriteLine();

            string nome    = LerTexto("  Nome da exposição");
            string curador = LerTexto("  Curador responsável");
            TipoExposicao tipo = LerEnum<TipoExposicao>("  Tipo de exposição");

            try
            {
                var expo = _gestor.CriarExposicao(nome, curador, tipo, DateTime.Now);
                Sucesso($"Exposição criada! ID: {expo.Id}");
                Console.WriteLine($"  {expo}");
                Console.WriteLine();

                Console.WriteLine("  Peças disponíveis no acervo:");
                foreach (var p in _gestor.Pecas.Where(p => p.EstaDisponivel()))
                    Console.WriteLine($"  {p}");

                Console.WriteLine();
                Console.WriteLine("  Associe peças à exposição (introduza 0 para terminar):");
                while (true)
                {
                    int pecaId = LerInteiro("  ID da peça (0 para terminar)");
                    if (pecaId == 0) break;

                    try
                    {
                        _gestor.AdicionarPecaAExposicao(expo.Id, pecaId);
                        var p = _gestor.ObterPecaPorId(pecaId);
                        Sucesso($"Peça '{p?.Nome}' adicionada à exposição.");
                    }
                    catch (PecaNaoDisponivelException ex)
                    {
                        Erro($"[PecaNaoDisponivelException] {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Erro(ex.Message);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Erro($"Dados inválidos: {ex.Message}");
            }

            Pausa();
        }

        static void Teste3_VenderBilhete()
        {
            Titulo("TESTE 3 — Vender Bilhete");

            if (!_gestor.Exposicoes.Any(e => !e.Encerrada))
            {
                Console.WriteLine("  Não existem exposições activas. Execute o Teste 2 primeiro.");
                Pausa();
                return;
            }

            Console.WriteLine("  Dados do visitante:");
            Console.WriteLine();

            string nome      = LerTexto("  Nome do visitante");
            string contacto  = LerTexto("  Contacto (email ou telefone)");
            CategoriaBilhete cat = LerEnum<CategoriaBilhete>("  Categoria");

            var visitante = _gestor.RegistarVisitante(nome, contacto, cat);
            Sucesso($"Visitante registado. ID: {visitante.Id}");
            Console.WriteLine();

            Console.WriteLine("  Exposições activas:");
            foreach (var e in _gestor.Exposicoes.Where(e => !e.Encerrada))
                Console.WriteLine($"  {e}");

            int expId = LerInteiro("  ID da exposição");

            try
            {
                var bilhete = _gestor.VenderBilhete(visitante.Id, expId);
                Sucesso("Bilhete emitido com sucesso!");
                Console.WriteLine($"  {bilhete}");

                Console.WriteLine();
                Console.WriteLine("  >> Teste da excepção ExposicaoEncerradaException:");
                Console.WriteLine("  Tentar vender bilhete para uma exposição encerrada...");

                var expoTemp = _gestor.CriarExposicao("Expo Encerrada (Teste)", "Sistema", TipoExposicao.Temporaria, DateTime.Now);
                _gestor.EncerrarExposicao(expoTemp.Id);

                var visitanteTemp = _gestor.RegistarVisitante("Visitante Teste", "teste@test.com", CategoriaBilhete.Adulto);
                try
                {
                    _gestor.VenderBilhete(visitanteTemp.Id, expoTemp.Id);
                }
                catch (ExposicaoEncerradaException ex)
                {
                    Aviso($"[ExposicaoEncerradaException] {ex.Message}");
                }
            }
            catch (ExposicaoEncerradaException ex)
            {
                Erro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                Erro(ex.Message);
            }

            Pausa();
        }

        static void Teste4_VisitaGuiada()
        {
            Titulo("TESTE 4 — Registar Visita Guiada");

            if (!_gestor.Exposicoes.Any(e => !e.Encerrada))
            {
                Console.WriteLine("  Não existem exposições activas. Execute o Teste 2 primeiro.");
                Pausa();
                return;
            }

            Console.WriteLine("  Dados da visita guiada:");
            Console.WriteLine();

            string guia = LerTexto("  Nome do guia responsável");

            Console.WriteLine("  Exposições activas:");
            foreach (var e in _gestor.Exposicoes.Where(e => !e.Encerrada))
                Console.WriteLine($"  {e}");

            int expId      = LerInteiro("  ID da exposição");
            int capacidade = LerInteiro("  Capacidade máxima do grupo");

            try
            {
                var visita = _gestor.RegistarVisitaGuiada(guia, expId, DateTime.Now.AddDays(1), capacidade);
                Sucesso($"Visita guiada registada! ID: {visita.Id}");
                Console.WriteLine($"  {visita}");
                Console.WriteLine();

                if (_gestor.Visitantes.Any())
                {
                    Console.WriteLine("  Visitantes registados:");
                    foreach (var v in _gestor.Visitantes)
                        Console.WriteLine($"  {v}");

                    Console.WriteLine();
                    Console.WriteLine("  Inscreva visitantes na visita (introduza 0 para terminar):");
                    while (true)
                    {
                        int vid = LerInteiro("  ID do visitante (0 para terminar)");
                        if (vid == 0) break;

                        try
                        {
                            _gestor.InscreverVisitanteEmVisita(visita.Id, vid);
                            var v = _gestor.ObterVisitantePorId(vid);
                            Sucesso($"'{v?.Nome}' inscrito na visita.");
                        }
                        catch (CapacidadeExcedidaException ex)
                        {
                            Aviso($"[CapacidadeExcedidaException] {ex.Message}");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Erro(ex.Message);
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("  >> Teste da excepção CapacidadeExcedidaException:");
                    Console.WriteLine($"  Tentar inscrever além da capacidade máxima ({capacidade})...");
                    try
                    {
                        for (int i = visita.NumeroParticipantes; i <= capacidade; i++)
                        {
                            var vExtra = _gestor.RegistarVisitante($"Teste Extra {i}", $"extra{i}@test.com", CategoriaBilhete.Adulto);
                            _gestor.InscreverVisitanteEmVisita(visita.Id, vExtra.Id);
                        }
                    }
                    catch (CapacidadeExcedidaException ex)
                    {
                        Aviso($"[CapacidadeExcedidaException] {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("  Sem visitantes registados. Execute o Teste 3 para registar visitantes.");
                }
            }
            catch (ExposicaoEncerradaException ex)
            {
                Erro($"[ExposicaoEncerradaException] {ex.Message}");
            }
            catch (Exception ex)
            {
                Erro(ex.Message);
            }

            Pausa();
        }

        static void Teste5_Emprestimo()
        {
            Titulo("TESTE 5 — Controlar Empréstimo");

            var disponíveis = _gestor.Pecas.Where(p => p.EstaDisponivel()).ToList();
            if (!disponíveis.Any())
            {
                Console.WriteLine("  Não existem peças disponíveis. Execute o Teste 1 primeiro.");
                Pausa();
                return;
            }

            Console.WriteLine("  Peças disponíveis para empréstimo:");
            foreach (var p in disponíveis)
                Console.WriteLine($"  {p}");

            Console.WriteLine();
            int pecaId = LerInteiro("  ID da peça a emprestar");
            string museu = LerTexto("  Museu de destino");

            Console.Write("  Data prevista de regresso (dd/MM/yyyy): ");
            string dataStr = (Console.ReadLine() ?? "").Trim();
            if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out DateTime dataRegresso))
            {
                Erro("Formato de data inválido. Use dd/MM/yyyy.");
                Pausa();
                return;
            }

            try
            {
                _gestor.IniciarEmprestimo(pecaId, museu, dataRegresso);
                var peca = _gestor.ObterPecaPorId(pecaId);
                Sucesso($"Empréstimo iniciado para '{peca?.Nome}' → {museu}");
                Console.WriteLine($"  Disponível: {peca?.EstaDisponivel()} (esperado: False)");

                Console.WriteLine();
                Console.WriteLine("  >> Teste da excepção PecaNaoDisponivelException:");
                Console.WriteLine($"  Tentar emprestar '{peca?.Nome}' novamente enquanto já está emprestada...");
                try
                {
                    _gestor.IniciarEmprestimo(pecaId, "Outro Museu", DateTime.Now.AddMonths(1));
                }
                catch (PecaNaoDisponivelException ex)
                {
                    Aviso($"[PecaNaoDisponivelException] {ex.Message}");
                }

                Console.WriteLine();
                Console.Write("  Registar devolução agora? (s/n): ");
                string resp = (Console.ReadLine() ?? "").Trim().ToLower();
                if (resp == "s")
                {
                    _gestor.DevolverEmprestimo(pecaId);
                    Sucesso($"Devolução registada. Disponível: {peca?.EstaDisponivel()} (esperado: True)");
                }
            }
            catch (PecaNaoDisponivelException ex)
            {
                Erro($"[PecaNaoDisponivelException] {ex.Message}");
            }
            catch (Exception ex)
            {
                Erro(ex.Message);
            }

            Pausa();
        }

        static void Teste6_Relatorio()
        {
            Titulo("TESTE 6 — Relatório de Visitação");

            if (!_gestor.Bilhetes.Any())
            {
                Console.WriteLine("  Sem bilhetes registados. Execute o Teste 3 primeiro.");
                Pausa();
                return;
            }

            Console.WriteLine("  Tipo de relatório:");
            Console.WriteLine("    1. Geral (todos os registos)");
            Console.WriteLine("    2. Por período");
            Console.WriteLine("    3. Por exposição");
            Console.WriteLine("    4. Por período e exposição");
            Console.Write("  Opção: ");
            string op = (Console.ReadLine() ?? "").Trim();

            DateTime? inicio = null, fim = null;
            int? expId = null;

            if (op == "2" || op == "4")
            {
                Console.Write("  Data de início (dd/MM/yyyy): ");
                string di = (Console.ReadLine() ?? "").Trim();
                if (DateTime.TryParseExact(di, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime dti))
                    inicio = dti;

                Console.Write("  Data de fim (dd/MM/yyyy): ");
                string df = (Console.ReadLine() ?? "").Trim();
                if (DateTime.TryParseExact(df, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime dtf))
                    fim = dtf;
            }

            if (op == "3" || op == "4")
            {
                Console.WriteLine("  Exposições disponíveis:");
                foreach (var e in _gestor.Exposicoes)
                    Console.WriteLine($"  {e}");
                expId = LerInteiro("  ID da exposição");
            }

            _gestor.GerarRelatorio(inicio, fim, expId);
            Pausa();
        }

        static void ExecutarTodosTestes()
        {
            Console.WriteLine();
            Console.WriteLine("  Os testes serão executados em sequência.");
            Console.WriteLine("  Cada teste pede os dados necessários.");
            Console.WriteLine();
            Console.Write("  Continuar? (s/n): ");
            if ((Console.ReadLine() ?? "").Trim().ToLower() != "s") return;

            Teste1_CatalogarPeca();
            Teste2_CriarExposicao();
            Teste3_VenderBilhete();
            Teste4_VisitaGuiada();
            Teste5_Emprestimo();
            Teste6_Relatorio();

            Console.WriteLine();
            Sucesso("Todos os testes concluídos.");
            Pausa();
        }

        static void MostrarMenuPrincipal()
        {
            Console.WriteLine();
            Console.WriteLine("  ┌─────────────────────────────────────────┐");
            Console.WriteLine("  │         SISTEMA DE GESTÃO DE MUSEU      │");
            Console.WriteLine("  ├─────────────────────────────────────────┤");
            Console.WriteLine("  │  1. Executar Testes                     │");
            Console.WriteLine("  │  2. Menu Principal (modo livre)         │");
            Console.WriteLine("  │  0. Sair                                │");
            Console.WriteLine("  └─────────────────────────────────────────┘");
            Console.Write("  Opção: ");
        }

        static void MostrarCabecalho()
        {
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║      MUSEU NACIONAL - GESTÃO INTERNA     ║");
            Console.WriteLine("  ║         POO II com C# - Projecto 23      ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
        }

        static string LerTexto(string prompt)
        {
            string val;
            do
            {
                Console.Write($"{prompt}: ");
                val = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(val))
                    Erro("Campo obrigatório.");
            } while (string.IsNullOrWhiteSpace(val));
            return val;
        }

        static int LerInteiro(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int val)) return val;
                Erro("Introduza um número inteiro válido.");
            }
        }

        static T LerEnum<T>(string prompt) where T : struct, Enum
        {
            var valores = (T[])Enum.GetValues(typeof(T));
            Console.WriteLine($"{prompt}:");
            for (int i = 0; i < valores.Length; i++)
                Console.WriteLine($"    {i + 1}. {valores[i]}");
            while (true)
            {
                Console.Write("  Escolha: ");
                if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= valores.Length)
                    return valores[idx - 1];
                Erro("Opção inválida.");
            }
        }

        static void Titulo(string msg)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ── {msg} ──");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void Sucesso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✓ {msg}");
            Console.ResetColor();
        }

        static void Erro(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ {msg}");
            Console.ResetColor();
        }

        static void Aviso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ {msg}");
            Console.ResetColor();
        }

        static void Pausa()
        {
            Console.WriteLine();
            Console.Write("  Pressione ENTER para continuar...");
            Console.ReadLine();
        }
    }
}
