/*
    Enemy.cs
    --------
    Representa um inimigo do jogo (Exercicio 6 - Batalha contra Monstro).

    Enemy NAO herda de Player de proposito. Heranca se le como "e um":
    "Mago e um Player" faz sentido, "Inimigo e um Player" nao faz.
    As duas classes so se parecem porque guardam dados semelhantes.
*/
public class Enemy
{
    public string name { get; set; } = "";
    public int health { get; set; }
    public int maxHealth { get; set; }
    public int damage { get; set; }
    public int defense { get; set; }   // porcentagem de reducao de dano, igual a do Player

    // Recompensas que o jogador recebe ao derrotar este inimigo.
    // Ficam aqui, e nao no Player, porque o valor depende de QUAL inimigo
    // foi derrotado: um goblin da pouco, um dragao da muito.
    public int expReward { get; set; }
    public int goldReward { get; set; }

    /*
        Propriedade calculada: nao guarda valor, executa a conta toda vez
        que e lida. Equivale ao @property do Python.
        Por isso nao tem "set" - nao faz sentido atribuir valor ao
        resultado de uma conta.
    */
    public bool isAlive => health > 0;

    // Construtor
    public Enemy(string name, int health, int damage, int defense, int expReward, int goldReward)
    {
        this.name = name;
        this.health = health;
        this.maxHealth = health;
        this.damage = damage;
        this.defense = defense;
        this.expReward = expReward;
        this.goldReward = goldReward;
    }

    /*
        Mesma regra do Player: a defesa reduz uma PORCENTAGEM do dano.
        O codigo esta repetido de proposito para as duas classes ficarem
        independentes - unificar isso exigiria uma classe "Personagem" acima
        das duas, que e uma melhoria para depois da entrega.
    */
    public int ReceberDano(int dano)
    {
        int defesaEfetiva = this.defense;

        if (defesaEfetiva > 80)
        {
            defesaEfetiva = 80;
        }
        if (defesaEfetiva < 0)
        {
            defesaEfetiva = 0;
        }

        // Multiplica antes de dividir: (10 * 20) / 100 = 2.
        // Se fosse 10 * (20 / 100), o int descartaria o decimal e daria 0.
        int reducao = (dano * defesaEfetiva) / 100;

        int danoReal = dano - reducao;

        if (danoReal < 1)
        {
            danoReal = 1;
        }

        this.health = this.health - danoReal;

        if (this.health < 0)
        {
            this.health = 0;
        }

        return danoReal;
    }
}
