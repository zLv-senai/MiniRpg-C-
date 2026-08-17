using System;

/*
    Program.cs
    ----------
    Ponto de entrada do Mini RPG.

    Todo programa em C# precisa de um metodo Main: e por ele que a execucao
    comeca. Em Python o interpretador simplesmente le o arquivo de cima para
    baixo; aqui e obrigatorio existir um Main dentro de uma classe.

    EXERCICIO 10 - Mini RPG Completo.
    Este arquivo junta todos os exercicios num programa so:
      Ex 1  - vida e dano ............ Player.ReceberDano
      Ex 2  - coleta de moedas ....... Fase.VasculharRuinas
      Ex 3  - ataque critico ......... Combate.CalcularDano
      Ex 4  - niveis e experiencia ... Player.GanharExp
      Ex 5  - inventario ............. Player.AdicionarItem / MostrarInventario
      Ex 6  - batalha por turnos ..... Combate.Batalhar
      Ex 7  - loja ................... Loja.Abrir
      Ex 8  - missoes ................ GerenciadorMissoes
      Ex 9  - ranking ................ Ranking.Menu
      Ex 10 - integracao ............. este menu

    Desafios extras implementados:
      - Salvar progresso em arquivo texto ... SaveGame.cs
      - Diferentes tipos de inimigos ........ SortearInimigo
      - Magia e mana ........................ Player.recurso + CatalogoAtaques
*/
class Program
{
    // Sorteio usado para escolher qual inimigo aparece.
    private static Random sorteio = new Random();

    // CONTADORES do progresso do jogador, usados para checar as missoes.
    private static int inimigosDerrotados = 0;
    private static int itensComprados = 0;

    static void Main(string[] args)
    {
        // Faz o console do Windows exibir acentos corretamente.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("========================================");
        Console.WriteLine("              MINI RPG");
        Console.WriteLine("========================================");

        /*
            O "?" avisa que a variavel pode ser nula - acontece quando o
            jogador escolhe sair no menu inicial.
        */
        DadosDoJogo? dados = MenuInicial();

        if (dados == null)
        {
            Console.WriteLine("Ate a proxima!");
            return;   // encerra o Main, e com ele o programa
        }

        // Restaura o estado da partida (novo jogo ou save carregado).
        Player heroi = dados.heroi;
        List<Missao> missoes = dados.missoes;
        inimigosDerrotados = dados.inimigosDerrotados;
        itensComprados = dados.itensComprados;

        Console.WriteLine();
        heroi.MostrarStatus();

        MenuPrincipal(heroi, missoes);

        Console.WriteLine();
        Console.WriteLine("Obrigado por jogar!");
    }

    /*
        Menu de abertura: comecar do zero ou continuar um save.
        Devolve null se o jogador escolher sair.
    */
    static DadosDoJogo? MenuInicial()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("======= INICIO =======");
            Console.WriteLine("1 - Novo jogo");
            Console.WriteLine("2 - Carregar jogo salvo");
            Console.WriteLine("0 - Sair");
            Console.Write("Opcao: ");

            string opcao = Console.ReadLine() ?? "";

            if (opcao == "1")
            {
                Player novo = CriarPersonagem();

                // Partida nova: missoes zeradas e contadores em zero.
                return new DadosDoJogo(novo, GerenciadorMissoes.CriarMissoes(), 0, 0);
            }
            else if (opcao == "2")
            {
                DadosDoJogo? carregado = MenuCarregar();

                if (carregado != null)
                {
                    return carregado;
                }

                // Deu errado ou nao havia save: volta ao menu inicial.
                continue;
            }
            else if (opcao == "0")
            {
                return null;
            }
            else
            {
                Console.WriteLine("Opcao invalida.");
            }
        }
    }

    /*
        Mostra os saves existentes e carrega o escolhido.
        Devolve null se nao houver save ou se o jogador desistir.
    */
    static DadosDoJogo? MenuCarregar()
    {
        List<string> saves = SaveGame.ListarSaves();

        if (saves.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine("Nenhum jogo salvo encontrado.");
            Console.WriteLine("Comece um jogo novo - ele sera salvo ao fim da sessao.");
            return null;
        }

        Console.WriteLine();
        Console.WriteLine("=== JOGOS SALVOS ===");

        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {saves[i]}");
        }
        Console.WriteLine("0 - Voltar");
        Console.Write("Opcao: ");

        string entrada = Console.ReadLine() ?? "";

        int opcao;
        if (int.TryParse(entrada, out opcao) == false)
        {
            Console.WriteLine("Digite um numero valido.");
            return null;
        }

        if (opcao == 0)
        {
            return null;
        }

        // O menu comeca em 1, a lista comeca em 0.
        int indice = opcao - 1;

        if (indice < 0 || indice >= saves.Count)
        {
            Console.WriteLine("Essa opcao nao existe.");
            return null;
        }

        DadosDoJogo? dados = SaveGame.Carregar(saves[indice]);

        if (dados == null)
        {
            Console.WriteLine("Nao foi possivel ler esse save (arquivo corrompido?).");
            return null;
        }

        Console.WriteLine();
        Console.WriteLine($"Jogo de {dados.heroi.name} carregado!");

        return dados;
    }

    /*
        Menu principal do jogo. Fica repetindo ate o jogador escolher sair
        ou morrer em batalha.
    */
    static void MenuPrincipal(Player heroi, List<Missao> missoes)
    {
        bool jogando = true;

        while (jogando)
        {
            Console.WriteLine();
            Console.WriteLine("======= MENU PRINCIPAL =======");
            Console.WriteLine("1 - Ver status do personagem");
            Console.WriteLine("2 - Explorar (procurar inimigos)");
            Console.WriteLine("3 - Vasculhar as ruinas (procurar moedas)");
            Console.WriteLine("4 - Inventario e equipamento");
            Console.WriteLine("5 - Ir a loja");
            Console.WriteLine("6 - Ver missoes");
            Console.WriteLine("7 - Ranking local de pontuacoes");
            Console.WriteLine("0 - Salvar e sair");
            Console.Write("Opcao: ");

            string opcao = Console.ReadLine() ?? "";

            switch (opcao)
            {
                case "1":
                    Console.WriteLine();
                    heroi.MostrarStatus();
                    break;

                case "2":
                    Explorar(heroi);

                    // Se o heroi morreu, a sessao acaba.
                    if (heroi.isAlive == false)
                    {
                        EncerrarSessao(heroi, missoes, "Seu personagem foi derrotado");
                        jogando = false;
                    }
                    break;

                case "3":
                    Fase.VasculharRuinas(heroi);
                    break;

                case "4":
                    MenuInventario(heroi);
                    break;

                case "5":
                    // Abrir devolve quantos itens foram comprados nesta visita.
                    itensComprados = itensComprados + Loja.Abrir(heroi);
                    break;

                case "6":
                    GerenciadorMissoes.Mostrar(missoes);
                    break;

                case "7":
                    Ranking.Menu(heroi);
                    break;

                case "0":
                    EncerrarSessao(heroi, missoes, "Voce salvou e saiu");
                    jogando = false;   // condicao de parada do laco
                    break;

                default:
                    Console.WriteLine("Opcao invalida.");
                    break;
            }

            /*
                Depois de qualquer acao, checa se alguma missao foi cumprida.
                Fica fora do switch para nao repetir a chamada em cada case.
            */
            if (jogando)
            {
                GerenciadorMissoes.Verificar(heroi, missoes, inimigosDerrotados, itensComprados);
            }
        }
    }

    /*
        Inventario do jogador: ver, equipar e usar itens.
        DESAFIO EXTRA - sistema de equipamentos.
    */
    static void MenuInventario(Player heroi)
    {
        bool aberto = true;

        while (aberto)
        {
            Console.WriteLine();
            heroi.MostrarInventario();
            Console.WriteLine();
            heroi.MostrarEquipamento();

            Console.WriteLine();
            Console.WriteLine("1 - Equipar / usar um item");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opcao: ");

            string opcao = Console.ReadLine() ?? "";

            if (opcao == "0")
            {
                aberto = false;
                continue;
            }

            if (opcao != "1")
            {
                Console.WriteLine("Opcao invalida.");
                continue;
            }

            Console.Write("Numero da vaga do inventario (1 a 5): ");
            string entrada = Console.ReadLine() ?? "";

            int vaga;
            if (int.TryParse(entrada, out vaga) == false)
            {
                Console.WriteLine("Digite um numero valido.");
                continue;
            }

            // O menu mostra de 1 a 5, o vetor vai de 0 a 4.
            int indice = vaga - 1;

            if (indice < 0 || indice >= heroi.inventario.Length)
            {
                Console.WriteLine("Essa vaga nao existe.");
                continue;
            }

            if (heroi.inventario[indice] == "")
            {
                Console.WriteLine("Essa vaga esta vazia.");
                continue;
            }

            /*
                O inventario guarda so o NOME do item. Aqui buscamos o objeto
                completo no catalogo da loja para saber bonus, slot e classe.
            */
            Item? item = Loja.BuscarPorNome(heroi.inventario[indice]);

            if (item == null)
            {
                Console.WriteLine("Item desconhecido.");
                continue;
            }

            if (item.slot == "Consumivel")
            {
                heroi.UsarConsumivel(item, indice);
            }
            else
            {
                heroi.Equipar(item);
            }
        }
    }

    /*
        Fecha a sessao de jogo: salva o progresso, calcula a pontuacao final,
        grava no ranking local e mostra o resultado.

        Este metodo existe para o encerramento acontecer SEMPRE do mesmo jeito,
        seja por morte ou por saida pelo menu. Se o codigo estivesse copiado
        nos dois lugares, um dia a gente mudaria um e esqueceria o outro.
    */
    static void EncerrarSessao(Player heroi, List<Missao> missoes, string motivo)
    {
        int pontuacaoFinal = Ranking.CalcularPontuacao(heroi);

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine($"  FIM DE SESSAO - {motivo}");
        Console.WriteLine("========================================");
        heroi.MostrarStatus();
        Console.WriteLine();
        Console.WriteLine($"Pontuacao final: {pontuacaoFinal} pontos");
        Console.WriteLine($"  (nivel {heroi.level} x 100) + {heroi.gold} moedas + {heroi.exp} EXP");

        /*
            Se o personagem morreu, o save guarda ele com 1 de vida em vez
            de 0 - assim da para continuar a partida em vez de carregar um
            heroi morto que nao consegue fazer nada.
        */
        if (heroi.isAlive == false)
        {
            heroi.health = 1;
            Console.WriteLine("Seu personagem foi resgatado com 1 de vida no save.");
        }

        SaveGame.Salvar(heroi, missoes, inimigosDerrotados, itensComprados);
        Console.WriteLine($"Progresso salvo em saves/{SaveGame.LimparNome(heroi.name)}.txt");

        // Registro automatico no ranking local.
        Ranking.Salvar(heroi.name, pontuacaoFinal);
        Console.WriteLine("Pontuacao registrada no ranking local.");

        Ranking.Mostrar(pontuacaoFinal);
    }

    /*
        O heroi explora a regiao, encontra um inimigo, luta,
        e recebe as recompensas se vencer.
    */
    static void Explorar(Player heroi)
    {
        Enemy inimigo = SortearInimigo();

        Console.WriteLine();
        Console.WriteLine($"Voce explora a regiao... e um {inimigo.name} aparece!");

        bool venceu = Combate.Batalhar(heroi, inimigo);

        if (venceu)
        {
            inimigosDerrotados = inimigosDerrotados + 1;

            // Drop do inimigo: fonte principal de moedas do jogo.
            heroi.gold = heroi.gold + inimigo.goldReward;
            Console.WriteLine($"{inimigo.name} dropou {inimigo.goldReward} moedas! Total: {heroi.gold}");

            // EXERCICIO 4 - ganho de experiencia e subida de nivel
            heroi.GanharExp(inimigo.expReward);
        }
    }

    /*
        Devolve um inimigo aleatorio.
        Cada um tem vida, dano, defesa e recompensas proprias.
    */
    static Enemy SortearInimigo()
    {
        int numero = sorteio.Next(1, 4);   // sorteia 1, 2 ou 3

        Enemy inimigo;

        switch (numero)
        {
            case 1:
                inimigo = new Enemy("Goblin", 60, 10, 5, 35, 25);
                break;

            case 2:
                inimigo = new Enemy("Lobo Selvagem", 70, 14, 10, 45, 20);
                break;

            default:
                inimigo = new Enemy("Esqueleto", 80, 15, 20, 55, 30);
                break;
        }

        // Cada inimigo recebe a propria lista de ataques.
        inimigo.ataques = CatalogoAtaques.DoInimigo(inimigo.name);

        return inimigo;
    }

    /*
        Pergunta nome e classe, e devolve o personagem pronto.

        O tipo de retorno e Player, mas o objeto devolvido pode ser um Mago,
        Guerreiro ou Arqueiro. Isso funciona porque todos eles HERDAM de
        Player - todo Mago e um Player. E o beneficio pratico da heranca.
    */
    static Player CriarPersonagem()
    {
        Console.Write("Digite o nome do seu personagem: ");
        string nome = Console.ReadLine() ?? "Heroi";

        // Se o jogador apertar Enter sem digitar nada, usa um nome padrao.
        if (nome == "")
        {
            nome = "Heroi";
        }

        Console.WriteLine();
        Console.WriteLine("Escolha a classe:");
        Console.WriteLine("1 - Mago      (vida 80  | ataque 25 | defesa 5%  | mana 50)");
        Console.WriteLine("2 - Guerreiro (vida 120 | ataque 15 | defesa 25% | stamina 40)");
        Console.WriteLine("3 - Arqueiro  (vida 100 | ataque 20 | defesa 12% | stamina 60)");
        Console.Write("Opcao: ");

        string opcao = Console.ReadLine() ?? "";

        /*
            switch e o equivalente ao match/case do Python, ou a uma
            sequencia de if/elif/else. Cada "case" precisa terminar com
            "return" ou "break", senao o C# nem compila.
        */
        switch (opcao)
        {
            case "1":
                Mago mago = new Mago(nome, 80, 25, 5, 1, 50);
                mago.ataques = CatalogoAtaques.DoMago();
                return mago;

            case "2":
                Guerreiro guerreiro = new Guerreiro(nome, 120, 15, 25, 1, 40);
                guerreiro.ataques = CatalogoAtaques.DoGuerreiro();
                return guerreiro;

            case "3":
                Arqueiro arqueiro = new Arqueiro(nome, 100, 20, 12, 1, 60);
                arqueiro.ataques = CatalogoAtaques.DoArqueiro();
                return arqueiro;

            default:
                Console.WriteLine("Opcao invalida! Voce sera um Guerreiro.");
                Guerreiro padrao = new Guerreiro(nome, 120, 15, 25, 1, 40);
                padrao.ataques = CatalogoAtaques.DoGuerreiro();
                return padrao;
        }
    }
}
