/*
    Loja.cs
    -------
    EXERCICIO 7 - Loja do Jogo.
    O jogador gasta as moedas que juntou comprando equipamentos.
*/

// Um produto vendido na loja.
public class Item
{
    public string nome {get; set;} = "";
    public int preco {get; set;}

    public Item(string nome, int preco)
    {
        this.nome = nome;
        this.preco = preco;
    }
}

public class Loja
{
    /*
        Catalogo fixo da loja, com os precos que o enunciado pede.
        E uma List porque a loja pode ganhar itens novos depois -
        diferente do inventario do jogador, que tem 5 vagas fixas.
    */
    public static List<Item> catalogo = new List<Item>()
    {
        new Item("Espada", 100),
        new Item("Armadura", 150),
        new Item("Pocao", 50)
    };

    // Retorna quantos itens o jogador comprou nesta visita
    // (o Program usa esse numero para checar a missao "Comprar um item na loja").
    public static int Abrir(Player heroi)
    {
        // Laco da loja: fica aberta ate o jogador escolher sair (opcao 0).
        bool continuar = true;
        int comprados = 0;

        while (continuar)
        {
            Console.WriteLine();
            Console.WriteLine("=== LOJA ===");
            Console.WriteLine($"Suas moedas: {heroi.gold}");
            Console.WriteLine();

            /*
                Monta o menu a partir do catalogo, em vez de escrever
                as 3 linhas na mao. Se um item novo entrar na lista,
                o menu se atualiza sozinho.
            */
            for (int i = 0; i < catalogo.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {catalogo[i].nome} ({catalogo[i].preco} moedas)");
            }
            Console.WriteLine("0 - Sair da loja");
            Console.Write("Opcao: ");

            string entrada = Console.ReadLine() ?? "";
            int opcao;

            if (int.TryParse(entrada, out opcao) == false)
            {
                Console.WriteLine("Digite um numero valido.");
                continue;   // volta para o inicio do while sem executar o resto
            }

            if (opcao == 0)
            {
                continuar = false;   // condicao de parada do laco
                Console.WriteLine("Voce saiu da loja.");
                continue;
            }

            /*
                O menu mostra de 1 ate 3, mas o indice do catalogo vai de
                0 ate 2. Por isso o -1. Errar isso e o classico
                "index out of range".
            */
            int indice = opcao - 1;

            if (indice < 0 || indice >= catalogo.Count)
            {
                Console.WriteLine("Essa opcao nao existe.");
                continue;
            }

            Item escolhido = catalogo[indice];

            // Verifica saldo suficiente, como o enunciado pede.
            if (heroi.gold < escolhido.preco)
            {
                Console.WriteLine($"Saldo insuficiente! {escolhido.nome} custa {escolhido.preco} e voce tem {heroi.gold}.");
                continue;
            }

            // Tenta guardar no inventario ANTES de cobrar.
            // Se o inventario estiver cheio, o jogador nao perde as moedas.
            if (heroi.AdicionarItem(escolhido.nome) == false)
            {
                Console.WriteLine("Seu inventario esta cheio! Nao da para comprar mais nada.");
                continue;
            }

            heroi.gold = heroi.gold - escolhido.preco;
            comprados = comprados + 1;
            Console.WriteLine($"Voce comprou {escolhido.nome}! Moedas restantes: {heroi.gold}");
        }

        return comprados;
    }
}
