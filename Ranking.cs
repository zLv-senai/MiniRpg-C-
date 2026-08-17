/*
    Ranking.cs
    ----------
    EXERCICIO 9 - Ranking de Pontuacoes.
    Vetor de 10 pontuacoes, ordenado da maior para a menor, exibindo o Top 3.

    O ranking e LOCAL e PERSISTENTE: fica salvo no arquivo ranking.txt, ao lado
    do projeto. Isso significa que as pontuacoes continuam la depois de fechar
    o jogo - cobre tambem o desafio extra "salvar progresso em arquivo texto".

    A pontuacao do jogador e gravada AUTOMATICAMENTE quando a sessao termina,
    seja por morte do personagem ou por saida pelo menu. Quem faz essa chamada
    e o metodo Program.EncerrarSessao.

    Formato do arquivo (uma partida por linha):
        Nome;Pontuacao
*/
public class Ranking
{
    // const = valor fixo, definido aqui e nunca alterado durante a execucao.
    private const string ARQUIVO = "ranking.txt";

    /*
        Pontuacao do jogador, calculada a partir do progresso dele.
        Nivel vale muito, moedas e experiencia valem menos.
    */
    public static int CalcularPontuacao(Player heroi)
    {
        return (heroi.level * 100) + heroi.gold + heroi.exp;
    }

    // Menu do ranking.
    public static void Menu(Player heroi)
    {
        bool aberto = true;

        while (aberto)
        {
            int minhaPontuacao = CalcularPontuacao(heroi);

            Console.WriteLine();
            Console.WriteLine("=== RANKING LOCAL ===");
            Console.WriteLine($"Sua pontuacao atual: {minhaPontuacao}");
            Console.WriteLine($"  (nivel {heroi.level} x 100) + {heroi.gold} moedas + {heroi.exp} EXP");
            Console.WriteLine();
            Console.WriteLine("1 - Ver o ranking (Top 3 e lista completa)");
            Console.WriteLine("2 - Adicionar pontuacao de outro jogador");
            Console.WriteLine("0 - Voltar");
            Console.WriteLine();
            Console.WriteLine("(sua pontuacao e registrada sozinha ao fim da sessao)");
            Console.Write("Opcao: ");

            string opcao = Console.ReadLine() ?? "";

            switch (opcao)
            {
                case "1":
                    Mostrar(minhaPontuacao);
                    break;

                case "2":
                    AdicionarManual();
                    break;

                case "0":
                    aberto = false;
                    break;

                default:
                    Console.WriteLine("Opcao invalida.");
                    break;
            }
        }
    }

    /*
        Grava uma linha no fim do arquivo.
        AppendAllText cria o arquivo se ele ainda nao existir - por isso nao
        precisa checar antes.
    */
    public static void Salvar(string nome, int pontuacao)
    {
        // Ponto e virgula separa os campos, entao nao pode aparecer no nome.
        string nomeLimpo = nome.Replace(";", " ");

        File.AppendAllText(ARQUIVO, $"{nomeLimpo};{pontuacao}\n");
    }

    /*
        EXERCICIO 9 - "Solicitar as pontuacoes".
        Permite cadastrar a pontuacao de outro jogador na mao.
    */
    private static void AdicionarManual()
    {
        Console.Write("Nome do jogador: ");
        string nome = Console.ReadLine() ?? "";

        if (nome == "")
        {
            nome = "Anonimo";
        }

        Console.Write("Pontuacao: ");
        string entrada = Console.ReadLine() ?? "0";

        int pontos;
        if (int.TryParse(entrada, out pontos) == false)
        {
            Console.WriteLine("Valor invalido, nao foi registrado.");
            return;
        }

        Salvar(nome, pontos);
        Console.WriteLine($"{nome} registrado com {pontos} pontos.");
    }

    /*
        Le o arquivo, ordena e mostra o Top 3.
        destaquePontuacao serve so para marcar a linha do jogador atual.
    */
    public static void Mostrar(int destaquePontuacao)
    {
        // File.Exists evita quebrar na primeira execucao, quando o arquivo
        // ainda nao foi criado.
        if (File.Exists(ARQUIVO) == false)
        {
            Console.WriteLine();
            Console.WriteLine("Ainda nao ha pontuacoes registradas.");
            Console.WriteLine("Use a opcao 2 para registrar a sua.");
            return;
        }

        string[] linhas = File.ReadAllLines(ARQUIVO);

        // Listas temporarias, porque nao sabemos quantas partidas o arquivo tem.
        List<string> nomesLidos = new List<string>();
        List<int> pontosLidos = new List<int>();

        for (int i = 0; i < linhas.Length; i++)
        {
            if (linhas[i] == "")
            {
                continue;   // pula linha em branco
            }

            /*
                Split corta o texto no ponto e virgula.
                "Lv;350" vira o vetor { "Lv", "350" }.
                Equivale ao "Lv;350".split(";") do Python.
            */
            string[] partes = linhas[i].Split(';');

            if (partes.Length != 2)
            {
                continue;   // linha estragada, ignora
            }

            int valor;
            if (int.TryParse(partes[1], out valor) == false)
            {
                continue;
            }

            nomesLidos.Add(partes[0]);
            pontosLidos.Add(valor);
        }

        if (pontosLidos.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine("Ainda nao ha pontuacoes validas registradas.");
            return;
        }

        /*
            Copia para VETORES, que e o que o exercicio 9 pede.
            Os dois vetores andam juntos: a posicao 3 de "nomes" e o dono da
            posicao 3 de "pontos". Por isso, ao trocar um, precisa trocar o outro.
        */
        int total = pontosLidos.Count;
        string[] nomes = new string[total];
        int[] pontos = new int[total];

        for (int i = 0; i < total; i++)
        {
            nomes[i] = nomesLidos[i];
            pontos[i] = pontosLidos[i];
        }

        OrdenarDoMaiorParaOMenor(pontos, nomes);

        /*
            O VETOR DE 10 POSICOES que o enunciado do exercicio 9 pede.
            Ele existe sempre com 10 vagas - isso e exigencia do enunciado e
            e o tamanho que um vetor tem em C#: fixo, definido na criacao.

            O que muda e a EXIBICAO: as vagas que ainda nao tem dono nao
            aparecem na tela. Guardar 10 e mostrar 3 sao coisas diferentes.
        */
        int[] top10 = new int[10];
        string[] nomesTop10 = new string[10];

        // Quantas vagas estao realmente preenchidas (no maximo 10).
        int preenchidas = total;
        if (preenchidas > 10)
        {
            preenchidas = 10;
        }

        for (int i = 0; i < 10; i++)
        {
            if (i < preenchidas)
            {
                top10[i] = pontos[i];
                nomesTop10[i] = nomes[i];
            }
            else
            {
                // Vaga vazia: fica no vetor, mas nao vai para a tela.
                top10[i] = 0;
                nomesTop10[i] = "---";
            }
        }

        /*
            O Top 3 so mostra 3 linhas se existirem 3 jogadores.
            Com 1 jogador registrado, mostra 1 linha.
        */
        int quantasNoTop = preenchidas;
        if (quantasNoTop > 3)
        {
            quantasNoTop = 3;
        }

        Console.WriteLine();
        Console.WriteLine("--- TOP 3 ---");

        for (int i = 0; i < quantasNoTop; i++)
        {
            string marcador = "";

            if (top10[i] == destaquePontuacao)
            {
                marcador = "   <-- voce";
            }

            Console.WriteLine($"{i + 1}o lugar: {nomesTop10[i]} - {top10[i]} pontos{marcador}");
        }

        // Aviso de quantas vagas do Top 3 ainda estao livres.
        if (preenchidas < 3)
        {
            int faltam = 3 - preenchidas;
            Console.WriteLine($"({faltam} vaga(s) do Top 3 ainda sem dono)");
        }

        /*
            A lista completa so aparece quando ha mais gente que o Top 3.
            Com 1 ou 2 jogadores, repetir a mesma informacao seria inutil.
        */
        if (preenchidas > 3)
        {
            Console.WriteLine();
            Console.WriteLine("--- RANKING COMPLETO ---");

            for (int i = 0; i < preenchidas; i++)
            {
                Console.WriteLine($"{i + 1,2}o - {nomesTop10[i],-15} {top10[i],6} pontos");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Total de partidas registradas: {total}");
    }

    /*
        Ordena os dois vetores da MAIOR para a menor pontuacao.

        Este e o metodo da bolha (bubble sort): compara cada par de vizinhos
        e troca os dois de lugar quando estao fora de ordem. Repetindo isso
        varias vezes, o maior valor vai "subindo" ate o comeco do vetor.

        Existe o atalho pronto Array.Sort(...), mas o exercicio e de logica,
        entao a ordenacao esta escrita na mao.
    */
    public static void OrdenarDoMaiorParaOMenor(int[] pontos, string[] nomes)
    {
        // Laco de fora: quantas passadas dar pelo vetor.
        for (int i = 0; i < pontos.Length - 1; i++)
        {
            /*
                Laco de dentro: compara os vizinhos.
                O "- i" existe porque, a cada passada, o maior valor que
                faltava ja ficou na posicao certa - nao precisa comparar de novo.
            */
            for (int j = 0; j < pontos.Length - 1 - i; j++)
            {
                // Maior para menor: se o da frente for MENOR, troca.
                if (pontos[j] < pontos[j + 1])
                {
                    /*
                        A TROCA PRECISA DE UMA VARIAVEL AUXILIAR.

                        Se fizesse direto:
                            pontos[j]     = pontos[j + 1];
                            pontos[j + 1] = pontos[j];
                        a primeira linha ja teria APAGADO o valor antigo de
                        pontos[j], e a segunda copiaria o valor novo de volta.
                        Os dois ficariam iguais e um valor sumiria para sempre.

                        (Python permite o atalho a, b = b, a; C# nao tem isso.)
                    */
                    int auxiliarPonto = pontos[j];
                    pontos[j] = pontos[j + 1];
                    pontos[j + 1] = auxiliarPonto;

                    // O nome tem que acompanhar a pontuacao dele, senao o
                    // ranking mostraria o nome de um com a nota de outro.
                    string auxiliarNome = nomes[j];
                    nomes[j] = nomes[j + 1];
                    nomes[j + 1] = auxiliarNome;
                }
            }
        }
    }
}
