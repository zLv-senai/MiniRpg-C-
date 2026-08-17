/*
    Combat.cs
    ---------
    Tudo que envolve luta: os ataques especiais e o sistema de batalha.
    Cobre os Exercicios 3 (ataque critico) e 6 (batalha por turnos).
*/

// Um ataque especial que um personagem pode usar (custa mana ou stamina).
public class Ataques
{
    public string name {get; set;} = "";
    public int damage {get; set;}
    public int cost {get; set;}

    public Ataques(string name, int damage, int cost)
    {
        this.name = name;
        this.damage = damage;
        this.cost = cost;
    }
    
}

public class Combate
{
    /*
        "static" aqui significa: existe UM sorteio para o programa inteiro,
        nao um por objeto.

        Isso e importante. Se criassemos "new Random()" a cada ataque, varios
        sorteios criados no mesmo instante nasceriam com a mesma semente e
        devolveriam o MESMO numero - o jogo daria critico sempre ou nunca.
        Em Python o modulo random ja e global por padrao e esse problema
        nao aparece.
    */
    private static Random sorteio = new Random();

    /*
        EXERCICIO 3 - Ataque Critico.
        Sorteia um numero de 1 a 100. Se for maior que 80, o dano dobra.
        Ou seja: 20% de chance de critico (os numeros 81 ate 100).
    */
    public static int CalcularDano(int danoBase, string nomeAtacante)
    {
        // ATENCAO: Next(1, 101) sorteia de 1 ate 100.
        // O segundo numero e EXCLUSIVO - igual ao range(1, 101) do Python.
        int numeroSorteado = sorteio.Next(1, 101);

        if (numeroSorteado > 80)
        {
            Console.WriteLine($">>> ATAQUE CRITICO de {nomeAtacante}! (sorteou {numeroSorteado})");
            return danoBase * 2;
        }

        return danoBase;
    }

    /*
        EXERCICIO 6 - Batalha contra o Monstro.
        Heroi e inimigo se revezam ate um dos dois morrer.
        Retorna true se o heroi venceu, false se ele morreu.
    */
    public static bool Batalhar(Player heroi, Enemy inimigo)
    {
        Console.WriteLine();
        Console.WriteLine($"--- BATALHA: {heroi.name} ({heroi.health} HP) x {inimigo.name} ({inimigo.health} HP) ---");
        Console.WriteLine();

        int turno = 1;

        /*
            A condicao de parada tem os DOIS lados ligados por && (E).
            O laco so continua enquanto os dois estiverem vivos.
            Assim que um morre, a condicao vira falsa e o laco termina.
            Em Python seria: while heroi.is_alive and inimigo.is_alive:
        */
        while (heroi.isAlive && inimigo.isAlive)
        {
            Console.WriteLine($"[ Turno {turno} ]");

            // --- Vez do heroi ---
            int danoDoHeroi = CalcularDano(heroi.damage, heroi.name);

            // O proprio inimigo aplica a reducao pela defesa dele.
            int danoAplicado = inimigo.ReceberDano(danoDoHeroi);

            Console.WriteLine($"{heroi.name} atacou causando {danoAplicado} de dano.");
            Console.WriteLine($"{inimigo.name} HP: {inimigo.health}");

            /*
                Se o inimigo morreu agora, o "break" sai do laco na hora.
                Sem isso, um monstro com 0 de vida ainda revidaria neste turno.
            */
            if (!inimigo.isAlive)
            {
                break;
            }

            Console.WriteLine();

            // --- Vez do inimigo ---
            int danoRecebido = heroi.ReceberDano(inimigo.damage);
            Console.WriteLine($"{inimigo.name} atacou causando {danoRecebido} de dano.");
            Console.WriteLine($"{heroi.name} HP: {heroi.health}");
            Console.WriteLine();

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
}
