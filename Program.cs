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
      Ex 9  - ranking ................ Ranking.Registrar
      Ex 10 - integracao ............. este menu
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
        Console.WriteLine();

        // Cadastro do personagem
        Player heroi = CriarPersonagem();

        // Cadastro das 3 missoes (Exercicio 8)
        List<Missao> missoes = GerenciadorMissoes.CriarMissoes();

        Console.WriteLine();
        Console.WriteLine("Personagem criado:");
        heroi.MostrarStatus();

        MenuPrincipal(heroi, missoes);

        Console.WriteLine();
        Console.WriteLine("Obrigado por jogar!");
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
            Console.WriteLine("4 - Ver inventario");
            Console.WriteLine("5 - Ir a loja");
            Console.WriteLine("6 - Ver missoes");
            Console.WriteLine("7 - Ranking de pontuacoes");
            Console.WriteLine("0 - Sair do jogo");
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

                    // Se o heroi morreu, o jogo acaba.
                    if (heroi.isAlive == false)
                    {
                        jogando = false;
                    }
                    break;

                case "3":
                    Fase.VasculharRuinas(heroi);
                    break;

                case "4":
                    Console.WriteLine();
                    heroi.MostrarInventario();
                    break;

                case "5":
                    // Abrir devolve quantos itens foram comprados nesta visita.
                    itensComprados = itensComprados + Loja.Abrir(heroi);
                    break;

                case "6":
                    GerenciadorMissoes.Mostrar(missoes);
                    break;

                case "7":
                    Ranking.Registrar(heroi);
                    break;

                case "0":
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

            // ACUMULADOR de moedas
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

        switch (numero)
        {
            case 1:
                return new Enemy("Goblin", 60, 10, 5, 35, 25);

            case 2:
                return new Enemy("Lobo Selvagem", 70, 14, 10, 45, 20);

            default:
                return new Enemy("Esqueleto", 80, 15, 20, 55, 30);
        }
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
                return new Mago(nome, 80, 25, 5, 1, 50);

            case "2":
                return new Guerreiro(nome, 120, 15, 25, 1, 40);

            case "3":
                return new Arqueiro(nome, 100, 20, 12, 1, 60);

            default:
                Console.WriteLine("Opcao invalida! Voce sera um Guerreiro.");
                return new Guerreiro(nome, 120, 15, 25, 1, 40);
        }
    }
}
