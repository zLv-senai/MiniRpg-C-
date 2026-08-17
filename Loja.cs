/*
    Loja.cs
    -------
    EXERCICIO 7 - Loja do Jogo.
    O jogador gasta as moedas que juntou comprando equipamentos.

    Tambem cobre o desafio extra "Criar sistema de equipamentos":
    cada arma tem uma classe que pode empunha-la e da bonus de ataque
    ou defesa a quem equipar.
*/

// Um produto vendido na loja.
public class Item
{
    public string nome {get; set;} = "";
    public int preco {get; set;}

    /*
        Quem pode usar: "Todos", "Mago", "Guerreiro" ou "Arqueiro".
        E o que impede um Mago de sair empunhando espada longa.
    */
    public string classePermitida {get; set;} = "Todos";

    // Onde o item se encaixa: "Arma", "Armadura" ou "Consumivel".
    public string slot {get; set;} = "Arma";

    public int bonusAtaque {get; set;}
    public int bonusDefesa {get; set;}   // em pontos percentuais de reducao
    public int curaVida {get; set;}      // usado pelos consumiveis

    /*
        bonusCritico: pontos percentuais A MAIS de chance de critico.
        Lembre que no ataque o criterio e "sorteio > limiteCritico", entao
        para AUMENTAR a chance o codigo DIMINUI o limite. Um arco com
        bonusCritico 7 faz um golpe de limite 80 virar limite 73.

        bonusRecurso: aumenta a mana/stamina maxima de quem equipa.
    */
    public int bonusCritico {get; set;}
    public int bonusRecurso {get; set;}

    public Item(string nome, int preco, string classePermitida, string slot, int bonusAtaque, int bonusDefesa, int curaVida, int bonusCritico, int bonusRecurso)
    {
        this.nome = nome;
        this.preco = preco;
        this.classePermitida = classePermitida;
        this.slot = slot;
        this.bonusAtaque = bonusAtaque;
        this.bonusDefesa = bonusDefesa;
        this.curaVida = curaVida;
        this.bonusCritico = bonusCritico;
        this.bonusRecurso = bonusRecurso;
    }

    /*
        Diz se este personagem pode usar o item.
        SaveGame.IdentificarClasse usa o operador "is" para descobrir se o
        objeto e Mago, Guerreiro ou Arqueiro em tempo de execucao.
    */
    public bool PodeSerUsadoPor(Player heroi)
    {
        if (this.classePermitida == "Todos")
        {
            return true;
        }

        return SaveGame.IdentificarClasse(heroi) == this.classePermitida;
    }

    // Texto curto com o que o item faz, usado nos menus.
    public string Descricao()
    {
        string texto = "";

        if (this.bonusAtaque > 0)
        {
            texto = texto + $"+{this.bonusAtaque} ataque ";
        }

        if (this.bonusDefesa > 0)
        {
            texto = texto + $"+{this.bonusDefesa}% defesa ";
        }

        if (this.curaVida > 0)
        {
            texto = texto + $"cura {this.curaVida} de vida ";
        }

        if (this.bonusCritico > 0)
        {
            texto = texto + $"+{this.bonusCritico}% critico ";
        }

        if (this.bonusRecurso > 0)
        {
            texto = texto + $"+{this.bonusRecurso} de recurso ";
        }

        if (texto == "")
        {
            texto = "sem efeito ";
        }

        return texto.Trim();
    }
}

public class Loja
{
    /*
        Catalogo fixo da loja.

        Os tres primeiros itens (Espada 100, Armadura 150, Pocao 50) sao
        exatamente os que o enunciado do exercicio 7 pede. Os demais sao
        extensao do projeto.

        Cada arma reforca o estilo da sua classe:
          Arco   -> o Arqueiro ja vive de critico, entao ganha mais critico.
          Cajado -> o Mago depende de mana para os golpes caros, entao ganha mana.

        Parametros: nome, preco, classe, slot, atq, def, cura, critico, recurso
    */
    public static List<Item> catalogo = new List<Item>()
    {
        new Item("Espada",           100, "Guerreiro", "Arma",       5,  0,  0,  0,  0),
        new Item("Armadura",         150, "Todos",     "Armadura",   0, 10,  0,  0,  0),
        new Item("Pocao",             50, "Todos",     "Consumivel", 0,  0, 40,  0,  0),
        new Item("Arco",             120, "Arqueiro",  "Arma",       8,  0,  0,  7,  0),
        new Item("Cajado",           120, "Mago",      "Arma",       9,  0,  0,  0, 25),
        new Item("Espada Longa",     180, "Guerreiro", "Arma",      12,  0,  0,  0,  0),
        new Item("Espada e Escudo",  250, "Guerreiro", "Arma",       7, 10,  0,  0,  0)
    };

    /*
        Procura um item pelo nome. Devolve null se nao achar.
        Usado pelo SaveGame para reconstruir o equipamento ao carregar
        um jogo salvo - o arquivo guarda so o nome do item.
    */
    public static Item? BuscarPorNome(string nome)
    {
        for (int i = 0; i < catalogo.Count; i++)
        {
            if (catalogo[i].nome == nome)
            {
                return catalogo[i];
            }
        }

        return null;
    }

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
            Console.WriteLine($"Suas moedas: {heroi.gold}   |   Classe: {SaveGame.IdentificarClasse(heroi)}");
            Console.WriteLine();

            /*
                Monta o menu a partir do catalogo, em vez de escrever
                as linhas na mao. Se um item novo entrar na lista,
                o menu se atualiza sozinho.
            */
            for (int i = 0; i < catalogo.Count; i++)
            {
                Item item = catalogo[i];

                // Marca o que a classe do jogador nao pode empunhar.
                string aviso = "";
                if (item.PodeSerUsadoPor(heroi) == false)
                {
                    aviso = $"   >> BLOQUEADO, item somente para {item.classePermitida}";
                }

                Console.WriteLine($"{i + 1} - {item.nome} ({item.preco} moedas) - {item.Descricao()}{aviso}");
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
                O menu mostra de 1 em diante, mas o indice do catalogo
                comeca em 0. Por isso o -1. Errar isso e o classico
                "index out of range".
            */
            int indice = opcao - 1;

            if (indice < 0 || indice >= catalogo.Count)
            {
                Console.WriteLine("Essa opcao nao existe.");
                continue;
            }

            Item escolhido = catalogo[indice];

            // Regra de classe: um Mago nao empunha espada.
            if (escolhido.PodeSerUsadoPor(heroi) == false)
            {
                Console.WriteLine($"BLOQUEADO: {escolhido.nome} e um item somente para {escolhido.classePermitida}. Voce e {SaveGame.IdentificarClasse(heroi)}.");
                continue;
            }

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

            // Equipa na hora se o slot ainda estiver livre - evita o jogador
            // comprar uma arma e sair para a batalha sem ela.
            if (escolhido.slot == "Arma" && heroi.arma == null)
            {
                heroi.Equipar(escolhido);
            }
            else if (escolhido.slot == "Armadura" && heroi.armadura == null)
            {
                heroi.Equipar(escolhido);
            }
        }

        return comprados;
    }
}
