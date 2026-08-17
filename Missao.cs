/*
    Missao.cs
    ---------
    EXERCICIO 8 - Sistema de Missoes.

    O enunciado pede a classe com Nome e Concluida. Aqui ela tem tambem
    recompensas, para a missao valer alguma coisa dentro do jogo.
*/
public class Missao
{
    public string nome {get; set;} = "";
    public bool concluida {get; set;} = false;   // toda missao nasce nao concluida

    public int recompensaExp {get; set;}
    public int recompensaGold {get; set;}

    public Missao(string nome, int recompensaExp, int recompensaGold)
    {
        this.nome = nome;
        this.recompensaExp = recompensaExp;
        this.recompensaGold = recompensaGold;
    }
}

// Cuida da lista de missoes do jogo.
public class GerenciadorMissoes
{
    // Cadastra as 3 missoes que o enunciado pede.
    public static List<Missao> CriarMissoes()
    {
        List<Missao> missoes = new List<Missao>();

        missoes.Add(new Missao("Derrotar 1 inimigo", 50, 30));
        missoes.Add(new Missao("Juntar 100 moedas", 80, 0));
        missoes.Add(new Missao("Comprar um item na loja", 60, 20));

        return missoes;
    }

    // Exibe as missoes e quantas ja foram concluidas (Desafio Extra).
    public static void Mostrar(List<Missao> missoes)
    {
        Console.WriteLine();
        Console.WriteLine("=== MISSOES ===");

        // CONTADOR: comeca em zero, fora do laco.
        int concluidas = 0;

        for (int i = 0; i < missoes.Count; i++)
        {
            Missao m = missoes[i];

            /*
                Operador ternario: condicao ? valor_se_true : valor_se_false
                Equivale ao "valor_se_true if condicao else valor_se_false" do Python.
            */
            string situacao = m.concluida ? "[CONCLUIDA]" : "[pendente] ";

            Console.WriteLine($"{situacao} {m.nome}  (recompensa: {m.recompensaExp} EXP, {m.recompensaGold} moedas)");

            if (m.concluida)
            {
                concluidas = concluidas + 1;
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Missoes concluidas: {concluidas} de {missoes.Count}");
    }

    /*
        Verifica se alguma missao pendente foi cumprida agora e entrega a
        recompensa. Chamado depois de cada acao importante do jogo.

        inimigosDerrotados e itensComprados sao contadores mantidos no Program.
    */
    public static void Verificar(Player heroi, List<Missao> missoes, int inimigosDerrotados, int itensComprados)
    {
        for (int i = 0; i < missoes.Count; i++)
        {
            Missao m = missoes[i];

            // Missao ja concluida nao e verificada de novo.
            if (m.concluida)
            {
                continue;
            }

            bool cumpriu = false;

            if (m.nome == "Derrotar 1 inimigo" && inimigosDerrotados >= 1)
            {
                cumpriu = true;
            }
            else if (m.nome == "Juntar 100 moedas" && heroi.gold >= 100)
            {
                cumpriu = true;
            }
            else if (m.nome == "Comprar um item na loja" && itensComprados >= 1)
            {
                cumpriu = true;
            }

            if (cumpriu)
            {
                m.concluida = true;

                Console.WriteLine();
                Console.WriteLine($">>> MISSAO CONCLUIDA: {m.nome}");

                heroi.gold = heroi.gold + m.recompensaGold;
                heroi.GanharExp(m.recompensaExp);
            }
        }
    }
}
