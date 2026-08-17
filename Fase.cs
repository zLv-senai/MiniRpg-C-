/*
    Fase.cs
    -------
    EXERCICIO 2 - Sistema de Coleta de Moedas.

    IMPORTANTE - de onde vem o dinheiro no jogo:
      - A fonte PRINCIPAL de moedas e o drop dos inimigos derrotados
        (campo goldReward em Enemy.cs, entregue em Program.Batalha).
      - Esta exploracao das ruinas e uma atividade OPCIONAL do menu, e existe
        para cumprir o enunciado do exercicio 2, que pede um laco de 10 coletas
        com a quantidade informada pelo usuario.

    Na lore: o heroi vasculha 10 bais nas ruinas e anota quanto achou em cada um.
*/
public class Fase
{
    public static void VasculharRuinas(Player heroi)
    {
        Console.WriteLine();
        Console.WriteLine("=== RUINAS ABANDONADAS ===");
        Console.WriteLine("Voce encontra 10 bais empoeirados. Abra um por um.");
        Console.WriteLine();

        /*
            ACUMULADOR: comeca em zero, FORA do laco.
            Se fosse declarado dentro do for, ele voltaria a zero a cada
            volta e o total no fim seria sempre o valor do ultimo bau.
        */
        int totalColetado = 0;

        /*
            for de 1 ate 10: a variavel i comeca em 1, o laco roda enquanto
            i <= 10, e i++ acontece DEPOIS do corpo executar.
            Ordem real de cada volta: testa a condicao -> executa o corpo -> incrementa.
            Por isso o bau 10 e aberto, e o 11 nao existe.
        */
        for (int i = 1; i <= 10; i++)
        {
            Console.Write($"Bau {i}/10 - quantas moedas tinha dentro? ");
            string entrada = Console.ReadLine() ?? "0";

            /*
                int.TryParse tenta converter texto em numero.
                Devolve true se conseguiu e joga o numero em "quantidade".
                E o equivalente seguro ao int(entrada) do Python dentro de
                um try/except: se o usuario digitar "abc", nao quebra o programa.
            */
            int quantidade;
            if (int.TryParse(entrada, out quantidade) == false)
            {
                Console.WriteLine("  Valor invalido, contando como 0.");
                quantidade = 0;
            }

            if (quantidade < 0)
            {
                Console.WriteLine("  Nao existe moeda negativa, contando como 0.");
                quantidade = 0;
            }

            // O acumulador soma o valor novo ao que ja tinha.
            totalColetado = totalColetado + quantidade;
        }

        Console.WriteLine();
        Console.WriteLine($"Total encontrado nas ruinas: {totalColetado} moedas.");

        // DESAFIO EXTRA do exercicio 2: mensagem especial ao atingir 100.
        if (totalColetado >= 100)
        {
            Console.WriteLine("*** TESOURO! Voce juntou 100 moedas ou mais nesta exploracao! ***");
        }

        // Soma ao total que o jogador ja tinha (outro acumulador, agora no Player).
        heroi.gold = heroi.gold + totalColetado;
        Console.WriteLine($"Moedas totais de {heroi.name}: {heroi.gold}");
    }
}
