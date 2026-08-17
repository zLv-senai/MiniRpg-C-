/*
    Combat.cs
    ---------
    Tudo que envolve luta: os ataques, o catalogo de cada classe e o
    sistema de batalha por turnos.
    Cobre os Exercicios 3 (ataque critico) e 6 (batalha por turnos).
*/

/*
    Um ataque que um personagem pode usar.

    Sobre "damage": e o dano EXTRA do golpe, somado ao ataque do personagem.
    Dano final = personagem.damage + ataque.damage
    Assim, subir de nivel (+5 de ataque) melhora TODOS os ataques de uma vez.
*/
public class Ataques
{
    public string name {get; set;} = "";
    public int damage {get; set;}
    public int cost {get; set;}              // quanto gasta de mana/stamina

    public int nivelMinimo {get; set;}       // nivel em que o ataque e liberado

    /*
        limiteCritico: o sorteio vai de 1 a 100 e o golpe e critico quando
        o numero sorteado FOR MAIOR que este limite.

            limite 80 -> criticos em 81..100 -> 20% de chance
            limite 70 -> criticos em 71..100 -> 30% de chance

        Quanto MENOR o limite, MAIOR a chance de critico.
        O padrao 80 e o valor que o enunciado do exercicio 3 pede.
    */
    public int limiteCritico {get; set;}

    // Alguns golpes fracos devolvem um pouco de mana/stamina.
    public int recuperaRecurso {get; set;}

    public Ataques(string name, int damage, int cost, int nivelMinimo, int limiteCritico, int recuperaRecurso)
    {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
        this.nivelMinimo = nivelMinimo;
        this.limiteCritico = limiteCritico;
        this.recuperaRecurso = recuperaRecurso;
    }

    // Chance de critico em porcentagem, so para exibir no menu.
    public int ChanceCriticaEmPorcento => 100 - this.limiteCritico;
}

/*
    Catalogo de ataques do jogo.
    Cada classe tem 4 golpes, liberados nos niveis 1, 1, 3 e 5.
    O primeiro de cada lista custa 0 e recupera recurso: e a garantia de que
    o jogador nunca fica sem nenhuma acao possivel.
*/
public class CatalogoAtaques
{
    public static List<Ataques> DoMago()
    {
        List<Ataques> lista = new List<Ataques>();
        //                       nome                dano custo nivel limite recupera
        lista.Add(new Ataques("Golpe de Cajado",        0,    0,    1,    85,       6));
        lista.Add(new Ataques("Bola de Fogo",          18,   10,    1,    80,       0));
        lista.Add(new Ataques("Raio Congelante",       28,   18,    3,    78,       0));
        lista.Add(new Ataques("Meteoro",               48,   35,    5,    75,       0));
        return lista;
    }

    public static List<Ataques> DoGuerreiro()
    {
        List<Ataques> lista = new List<Ataques>();
        lista.Add(new Ataques("Ataque Basico",          0,    0,    1,    85,       6));
        lista.Add(new Ataques("Golpe Forte",           16,    8,    1,    80,       0));
        lista.Add(new Ataques("Investida",             26,   15,    3,    78,       0));
        lista.Add(new Ataques("Furia do Guerreiro",    44,   30,    5,    75,       0));
        return lista;
    }

    /*
        O Arqueiro causa menos dano bruto, mas os limites criticos dele sao
        mais baixos - ou seja, ele acerta critico com mais frequencia.
        Foi assim que a classe ganhou identidade propria.
    */
    public static List<Ataques> DoArqueiro()
    {
        List<Ataques> lista = new List<Ataques>();
        lista.Add(new Ataques("Tiro Rapido",            0,    0,    1,    80,       6));
        lista.Add(new Ataques("Flecha Perfurante",     15,    8,    1,    75,       0));
        lista.Add(new Ataques("Flecha Dupla",          23,   14,    3,    72,       0));
        lista.Add(new Ataques("Chuva de Flechas",      40,   28,    5,    68,       0));
        return lista;
    }

    // Ataques dos inimigos. O inimigo sorteia qual usar, sem menu.
    public static List<Ataques> DoInimigo(string nomeInimigo)
    {
        List<Ataques> lista = new List<Ataques>();

        if (nomeInimigo == "Goblin")
        {
            lista.Add(new Ataques("Adaga Enferrujada",  0, 0, 1, 85, 0));
            lista.Add(new Ataques("Mordida",            4, 0, 1, 80, 0));
        }
        else if (nomeInimigo == "Lobo Selvagem")
        {
            lista.Add(new Ataques("Arranhao",           0, 0, 1, 85, 0));
            lista.Add(new Ataques("Bote",               6, 0, 1, 75, 0));
        }
        else
        {
            lista.Add(new Ataques("Garra Ossea",        0, 0, 1, 85, 0));
            lista.Add(new Ataques("Investida Sombria",  8, 0, 1, 78, 0));
        }

        return lista;
    }
}

public class Combate
{
    /*
        "static" aqui significa: existe UM sorteio para o programa inteiro,
        nao um por objeto.

        Se criassemos "new Random()" a cada ataque, varios sorteios criados
        no mesmo instante nasceriam com a mesma semente e devolveriam o MESMO
        numero - o jogo daria critico sempre ou nunca. Em Python o modulo
        random ja e global por padrao e esse problema nao aparece.
    */
    private static Random sorteio = new Random();

    /*
        EXERCICIO 3 - Ataque Critico.
        Sorteia um numero de 1 a 100. Se for MAIOR que o limite, o dano dobra.
        Com limite 80, os criticos saem em 81..100, ou seja, 20% das vezes.
    */
    public static int CalcularDano(int danoBase, int limiteCritico, string nomeAtacante)
    {
        // ATENCAO: Next(1, 101) sorteia de 1 ate 100.
        // O segundo numero e EXCLUSIVO - igual ao range(1, 101) do Python.
        int numeroSorteado = sorteio.Next(1, 101);

        if (numeroSorteado > limiteCritico)
        {
            Console.WriteLine($"  >>> CRITICO de {nomeAtacante}! (sorteou {numeroSorteado}, precisava de mais que {limiteCritico})");
            return danoBase * 2;
        }

        return danoBase;
    }

    /*
        EXERCICIO 6 - Batalha contra o Monstro.
        Heroi e inimigo se revezam ate um dos dois morrer.
        Agora o jogador ESCOLHE qual ataque usar a cada turno.
        Retorna true se o heroi venceu, false se ele morreu.
    */
    public static bool Batalhar(Player heroi, Enemy inimigo)
    {
        Console.WriteLine();
        Console.WriteLine($"--- BATALHA: {heroi.name} x {inimigo.name} ---");

        int turno = 1;

        /*
            A condicao de parada tem os DOIS lados ligados por && (E).
            O laco so continua enquanto os dois estiverem vivos.
            Em Python seria: while heroi.is_alive and inimigo.is_alive:
        */
        while (heroi.isAlive && inimigo.isAlive)
        {
            Console.WriteLine();
            Console.WriteLine($"===== TURNO {turno} =====");
            Console.WriteLine($"{heroi.name}: {heroi.health}/{heroi.maxHealth} HP | {heroi.recursoNome}: {heroi.recurso}/{heroi.recursoMaxTotal}");
            Console.WriteLine($"{inimigo.name}: {inimigo.health}/{inimigo.maxHealth} HP");

            // --- Vez do heroi: ele escolhe o ataque ---
            Ataques escolhido = EscolherAtaque(heroi);

            // Paga o custo do golpe.
            heroi.recurso = heroi.recurso - escolhido.cost;

            // Golpes fracos devolvem um pouco de recurso, sem passar do maximo.
            heroi.recurso = heroi.recurso + escolhido.recuperaRecurso;
            if (heroi.recurso > heroi.recursoMaxTotal)
            {
                heroi.recurso = heroi.recursoMaxTotal;
            }

            // Dano final = ataque do personagem + dano extra do golpe.
            int danoBaseHeroi = heroi.ataqueTotal + escolhido.damage;   // ataqueTotal ja inclui a arma

            Console.WriteLine();
            Console.WriteLine($"{heroi.name} usa {escolhido.name}!");

            /*
                O equipamento diminui o limite do critico.
                Criterio e "sorteio > limite", entao limite menor = mais criticos.
                O piso de 50 impede um equipamento futuro de zerar o limite e
                deixar o jogo dando critico em 100% das vezes.
            */
            int limiteComEquipamento = escolhido.limiteCritico - heroi.criticoTotal;
            if (limiteComEquipamento < 50)
            {
                limiteComEquipamento = 50;
            }

            int danoDoHeroi = CalcularDano(danoBaseHeroi, limiteComEquipamento, heroi.name);

            // O proprio inimigo aplica a reducao pela defesa dele.
            int danoAplicado = inimigo.ReceberDano(danoDoHeroi);

            Console.WriteLine($"  {inimigo.name} perdeu {danoAplicado} de vida. HP: {inimigo.health}/{inimigo.maxHealth}");

            /*
                Se o inimigo morreu agora, o "break" sai do laco na hora.
                Sem isso, um monstro com 0 de vida ainda revidaria neste turno.
            */
            if (inimigo.isAlive == false)
            {
                break;
            }

            // --- Vez do inimigo: ele sorteia o proprio ataque ---
            Ataques ataqueInimigo = SortearAtaqueDoInimigo(inimigo);
            int danoBaseInimigo = inimigo.damage + ataqueInimigo.damage;

            Console.WriteLine();
            Console.WriteLine($"{inimigo.name} usa {ataqueInimigo.name}!");

            int danoDoInimigo = CalcularDano(danoBaseInimigo, ataqueInimigo.limiteCritico, inimigo.name);
            int danoRecebido = heroi.ReceberDano(danoDoInimigo);

            Console.WriteLine($"  {heroi.name} perdeu {danoRecebido} de vida. HP: {heroi.health}/{heroi.maxHealth}");

            turno = turno + 1;   // acumulador do contador de turnos
        }

        Console.WriteLine();

        // Quem saiu vivo do laco?
        if (heroi.isAlive)
        {
            Console.WriteLine($"*** {inimigo.name} foi derrotado! ***");
            return true;
        }
        else
        {
            // EXERCICIO 1 - "Game Over" quando a vida chega a 0 ou menos.
            Console.WriteLine("*** GAME OVER ***");
            Console.WriteLine($"{heroi.name} foi derrotado por {inimigo.name}.");
            return false;
        }
    }

    /*
        Mostra os ataques que o heroi pode usar e devolve o escolhido.

        Fica repetindo ate o jogador escolher algo valido - por isso o
        while (true) com return la dentro: so sai do metodo quando ha escolha.
    */
    private static Ataques EscolherAtaque(Player heroi)
    {
        while (true)
        {
            /*
                Monta a lista do que esta LIBERADO pelo nivel atual.
                heroi.ataques guarda os 4 golpes da classe desde o inicio;
                o filtro por nivelMinimo e o que faz eles aparecerem aos poucos.
            */
            List<Ataques> disponiveis = new List<Ataques>();

            for (int i = 0; i < heroi.ataques.Count; i++)
            {
                if (heroi.ataques[i].nivelMinimo <= heroi.level)
                {
                    disponiveis.Add(heroi.ataques[i]);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Escolha seu ataque:");

            for (int i = 0; i < disponiveis.Count; i++)
            {
                Ataques a = disponiveis[i];

                string custoTexto;
                if (a.cost == 0)
                {
                    custoTexto = "sem custo";
                }
                else
                {
                    custoTexto = $"custa {a.cost} de {heroi.recursoNome}";
                }

                string extra = "";
                if (a.recuperaRecurso > 0)
                {
                    extra = $", recupera {a.recuperaRecurso}";
                }

                // Marca o que o jogador nao tem recurso para usar agora.
                string bloqueado = "";
                if (heroi.recurso < a.cost)
                {
                    bloqueado = "  [SEM " + heroi.recursoNome.ToUpper() + "]";
                }

                // Chance de critico ja somando o bonus do equipamento.
                int criticoExibido = a.ChanceCriticaEmPorcento + heroi.criticoTotal;
                if (criticoExibido > 50)
                {
                    criticoExibido = 50;
                }

                Console.WriteLine($"{i + 1} - {a.name}: dano {heroi.ataqueTotal + a.damage}, {custoTexto}{extra}, critico {criticoExibido}%{bloqueado}");
            }

            Console.Write("Opcao: ");
            string entrada = Console.ReadLine() ?? "";

            int opcao;
            if (int.TryParse(entrada, out opcao) == false)
            {
                Console.WriteLine("Digite um numero valido.");
                continue;   // volta ao inicio do while e mostra o menu de novo
            }

            // O menu mostra de 1 em diante, a lista comeca no indice 0.
            int indice = opcao - 1;

            if (indice < 0 || indice >= disponiveis.Count)
            {
                Console.WriteLine("Essa opcao nao existe.");
                continue;
            }

            Ataques escolhido = disponiveis[indice];

            if (heroi.recurso < escolhido.cost)
            {
                Console.WriteLine($"{heroi.recursoNome} insuficiente! Voce tem {heroi.recurso} e precisa de {escolhido.cost}.");
                continue;
            }

            return escolhido;   // unica saida do while
        }
    }

    // O inimigo escolhe um ataque no sorteio, sem menu.
    private static Ataques SortearAtaqueDoInimigo(Enemy inimigo)
    {
        // Inimigo sem ataques cadastrados: usa um golpe generico.
        if (inimigo.ataques.Count == 0)
        {
            return new Ataques("Ataque", 0, 0, 1, 80, 0);
        }

        int indice = sorteio.Next(0, inimigo.ataques.Count);
        return inimigo.ataques[indice];
    }
}
