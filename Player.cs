/* Criando a classe Player, para pegar os valores base que cada classe 
precisa ter para nao ter que ficar em cada classe de personagens pegando todos
os valores base, e sim herdando da classe Player. 
*/
    public class Player
    {
        public string name {get; set;} = "";
        public int health {get; set;}
        public int damage {get; set;}
        public int defense {get; set;}
        public int level {get; set;}
        public int exp {get; set;} = 0;
        public bool isAlive {get; set;} = (health > 0) ? true : false;
        

        // Constructor 

        public Player(string name, int health, int damage, int defense, int level)
        {
            this.name = name;
            this.health = health;
            this.damage = damage;
            this.defense = defense;
            this.level = level;

        }
    }
/* Primeira subclasse herdando os valores base com o ":" de player */
public class Mago : Player
{
    public int mana {get; set;}
    public List<Ataques> ataques {get; set;} = new List<Ataques>();

    public Mago(string name, int health, int damage, int defense, int level, int mana)
    /* colocando os valores base da classe Player*/
    : base(name, health, damage, defense, level) 
    {
        this.mana = mana;
    }
}

public class Guerreiro : Player
{
    public int stamina {get; set;}

    public Guerreiro(string name, int health, int damage, int defense, int level, int stamina)
    : base(name, health, damage, defense, level)
    {
        this.stamina = stamina;
    }
}

public class Arqueiro : Player
{
    public int stamina {get; set;}

    public Arqueiro(string name, int health, int damage, int defense, int level, int stamina)
    : base(name, health, damage, defense, level)
    {
        this.stamina = stamina;
    }
}