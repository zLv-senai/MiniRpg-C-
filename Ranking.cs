/*
    Ranking.cs
    ----------
    EXERCICIO 9 - Ranking de Pontuacoes.
    Vetor de 10 pontuacoes, ordenado da maior para a menor, exibindo o Top 3.
*/
public class Ranking
{
    /*
        Pontuacao do jogador atual, calculada a partir do progresso dele.
        Nivel vale muito, moedas e experiencia valem menos.
    */
    public static int CalcularPontuacao(Player heroi)
    {
        return (heroi.level * 100) + heroi.gold + heroi.exp;
    }

    public static void Registrar(Player heroi)
    {
        // VETOR de tamanho fixo: 10 vagas, como o enunciado pede.
        int[] pontuacoes = new int[10];

        int pontuacaoDoHeroi = CalcularPontuacao(heroi);

        Console.WriteLine();
        Console.WriteLine("=== RANKING DE PONTUACOES ===");
        Console.WriteLine($"Sua pontuacao: {pontuacaoDoHeroi}");
        Console.WriteLine($"  (nivel {heroi.level} x 100) + {heroi.gold} moedas + {heroi.exp} EXP");
        Console.WriteLine();
        Console.WriteLine("Digite as pontuacoes dos outros 9 jogadores:");

        // A vaga 0 ja e do jogador atual.
        pontuacoes[0] = pontuacaoDoHeroi;

        /*
            O laco comeca em 1, e nao em 0, porque a posicao 0 ja esta ocupada.
            Vai ate 9 porque pontuacoes.Length e 10 e o ultimo indice valido
            de um vetor de 10 posicoes e 9. Usar <= no lugar de < aqui daria
            IndexOutOfRangeException - erro em tempo de execucao, nao de compilacao.
        */
        for (int i = 1; i < pontuacoes.Length; i++)
        {
            Console.Write($"Jogador {i + 1}: ");
            string entrada = Console.ReadLine() ?? "0";

            int valor;
            if (int.TryParse(entrada, out valor) == false)
            {
                Console.WriteLine("  Valor invalido, registrando 0.");
                valor = 0;
            }

            pontuacoes[i] = valor;
        }

        OrdenarDoMaiorParaOMenor(pontuacoes);
        MostrarTop3(pontuacoes, pontuacaoDoHeroi);
    }

    /*
        Ordena o vetor da MAIOR para a menor pontuacao.

        Este e o metodo da bolha (bubble sort): compara cada par de vizinhos
        e troca os dois de lugar quando estao fora de ordem. Repetindo isso
        varias vezes, o maior valor vai "subindo" ate o comeco do vetor.

        Existe o atalho pronto Array.Sort(vetor) + Array.Reverse(vetor),
        mas o exercicio e de logica, entao a ordenacao esta escrita na mao.
    */
    public static void OrdenarDoMaiorParaOMenor(int[] vetor)
    {
        // Laco de fora: quantas passadas dar pelo vetor.
        for (int i = 0; i < vetor.Length - 1; i++)
        {
            /*
                Laco de dentro: compara os vizinhos.
                O "- i" existe porque, a cada passada, o maior valor que
                faltava ja ficou na posicao certa - nao precisa comparar de novo.
            */
            for (int j = 0; j < vetor.Length - 1 - i; j++)
            {
                // Maior para menor: se o da frente for MENOR, troca.
                if (vetor[j] < vetor[j + 1])
                {
                    /*
                        A TROCA PRECISA DE UMA VARIAVEL AUXILIAR.

                        Se fizesse direto:
                            vetor[j]     = vetor[j + 1];
                            vetor[j + 1] = vetor[j];
                        a primeira linha ja teria APAGADO o valor antigo de
                        vetor[j], e a segunda linha copiaria o valor novo de
                        volta. O resultado seria os dois iguais e um valor
                        perdido para sempre.

                        Guardar em "auxiliar" antes preserva o valor.
                        (Python permite o atalho a, b = b, a; C# nao tem isso.)
                    */
                    int auxiliar = vetor[j];
                    vetor[j] = vetor[j + 1];
                    vetor[j + 1] = auxiliar;
                }
            }
        }
    }

    // Exibe o Top 3, como o enunciado pede.
    public static void MostrarTop3(int[] pontuacoes, int pontuacaoDoHeroi)
    {
        Console.WriteLine();
        Console.WriteLine("--- TOP 3 ---");

        for (int i = 0; i < 3; i++)
        {
            string marcador = "";

            // Marca a linha do jogador atual, se ele estiver no top 3.
            if (pontuacoes[i] == pontuacaoDoHeroi)
            {
                marcador = "  <-- voce";
            }

            Console.WriteLine($"{i + 1}o lugar: {pontuacoes[i]} pontos{marcador}");
        }

        Console.WriteLine();
        Console.Write("Ranking completo: ");
        for (int i = 0; i < pontuacoes.Length; i++)
        {
            Console.Write(pontuacoes[i]);

            // Nao coloca virgula depois do ultimo.
            if (i < pontuacoes.Length - 1)
            {
                Console.Write(", ");
            }
        }
        Console.WriteLine();
    }
}
