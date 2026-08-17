/* Criando a classe Player, para pegar os valores base que cada classe 
precisa ter para nao ter que ficar em cada classe de personagens pegando todos
os valores base, e sim herdando da classe Player. 
*/
    public class Player
    {
        public string name {get; set;} = "";
        public int health {get; set;}
        public int maxHealth {get; set;}   // vida cheia, usada para curar e mostrar "50/100"
        public int damage {get; set;}
        public int defense {get; set;}
        public int level {get; set;}

        /*
            RECURSO = mana do Mago, stamina do Guerreiro e do Arqueiro.

            O campo mora aqui na Player, e nao nas subclasses, para o combate
            poder escrever "gasta recurso" UMA vez so. Se mana ficasse no Mago
            e stamina no Guerreiro, o combate precisaria perguntar
            "que tipo de personagem e esse?" antes de cada ataque.

            recursoNome guarda como esse recurso se chama na tela.
        */
        public int recurso {get; set;}
        public int recursoMax {get; set;}
        public string recursoNome {get; set;} = "Recurso";

        // Lista de ataques que o personagem conhece (Combat.cs define a classe Ataques).
        public List<Ataques> ataques {get; set;} = new List<Ataques>();

        /*
            EXERCICIO 5 - Inventario de Itens.
            Vetor de 5 posicoes, criado ja preenchido com texto vazio.

            Vetor (array) tem tamanho FIXO: new string[5] sao 5 vagas para
            sempre. Diferente da List<> usada em "ataques", que cresce sozinha.
            O enunciado pede vetor justamente porque o inventario e limitado.
            Em Python nao existe essa separacao - lista serve para os dois casos.
        */
        public string[] inventario {get; set;} = new string[5] { "", "", "", "", "" };

        // ACUMULADORES: comecam em zero e vao somando ao longo do jogo.
        public int exp {get; set;} = 0;    // Exercicio 4 - experiencia
        public int gold {get; set;} = 0;   // Exercicio 2 e 7 - moedas

        /*
            EXERCICIO 4 - quanta EXP falta para o proximo nivel.

            Formula: 100 x nivel atual.
              Nivel 1 -> 2 custa 100   (o valor que o enunciado pede)
              Nivel 2 -> 3 custa 200
              Nivel 3 -> 4 custa 300

            Como e propriedade calculada, o custo se ajusta sozinho assim
            que o nivel muda - nao existe variavel para esquecer de atualizar.
        */
        public int expParaSubir => 100 * this.level;

        /* Propriedade calculada: recalcula "health > 0" toda vez que e lida.
           Antes era {get; set;} com inicializador, que dava erro CS0236 -
           inicializador nao pode ler outra propriedade de instancia. */
        public bool isAlive => health > 0;

        // Constructor 

        public Player(string name, int health, int damage, int defense, int level)
        {
            this.name = name;
            this.health = health;
            this.maxHealth = health;   // ao nascer, a vida atual e a vida maxima
            this.damage = damage;
            this.defense = defense;
            this.level = level;

        }

        /*
            EXERCICIO 1 - Sistema de Vida do Personagem.
            Aplica dano ao personagem, descontando a defesa.
            Retorna quanto de dano realmente entrou, para poder exibir na tela.
        */
        public int ReceberDano(int dano)
        {
            /*
                A defesa e uma PORCENTAGEM de reducao.
                Defesa 20 significa "recebo 20% menos dano".

                Trava de seguranca: acima de 80% o personagem ficaria
                praticamente imortal, entao a defesa util para no 80.
            */
            int defesaEfetiva = this.defense;

            if (defesaEfetiva > 80)
            {
                defesaEfetiva = 80;
            }
            if (defesaEfetiva < 0)
            {
                defesaEfetiva = 0;
            }

            /*
                ATENCAO - ARMADILHA DO C#:
                dividir dois int descarta o decimal. Entao

                    dano * (defesaEfetiva / 100)

                daria SEMPRE ZERO, porque 20 / 100 = 0 em int (nao 0.2).
                O programa compilaria e rodaria, e a defesa simplesmente
                nao funcionaria - um erro silencioso, dificil de achar.

                A solucao e MULTIPLICAR ANTES DE DIVIDIR:
                (10 * 20) / 100  ->  200 / 100  ->  2
            */
            int reducao = (dano * defesaEfetiva) / 100;

            int danoReal = dano - reducao;

            // Um ataque nunca cura nem zera: tira no minimo 1 de vida.
            // Se pudesse dar 0, um inimigo fraco bateria eternamente sem
            // tirar vida e a batalha nunca terminaria.
            if (danoReal < 1)
            {
                danoReal = 1;
            }

            this.health = this.health - danoReal;

            // Vida nao fica negativa: -15 de vida seria estranho de exibir.
            if (this.health < 0)
            {
                this.health = 0;
            }

            return danoReal;
        }

        /*
            EXERCICIO 4 - Sistema de Niveis.
            Soma a experiencia ganha e sobe de nivel a cada 100 pontos.
        */
        public void GanharExp(int quantidade)
        {
            // ACUMULADOR: o valor novo depende do valor antigo.
            this.exp = this.exp + quantidade;
            Console.WriteLine($"{this.name} ganhou {quantidade} de EXP. Total: {this.exp}/{this.expParaSubir}");

            /*
                Por que WHILE e nao IF?
                Se o jogador ganhar 250 de EXP de uma vez, o IF subiria
                UM nivel so e sobrariam 150 pontos parados. O WHILE
                repete ate a sobra ficar abaixo do necessario.

                Repare que expParaSubir e recalculado a cada volta, porque
                e propriedade calculada: assim que o nivel sobe, o custo
                do proximo nivel ja muda dentro do proprio laco.
            */
            while (this.exp >= this.expParaSubir)
            {
                this.exp = this.exp - this.expParaSubir;   // guarda a sobra
                SubirNivel();
            }
        }

        // Aumenta os atributos e cura o personagem por completo.
        public void SubirNivel()
        {
            this.level = this.level + 1;

            this.maxHealth = this.maxHealth + 20;
            this.health = this.maxHealth;      // subir de nivel cura tudo

            this.damage = this.damage + 5;
            this.defense = this.defense + 3;   // +3 pontos percentuais de reducao

            this.recursoMax = this.recursoMax + 10;
            this.recurso = this.recursoMax;

            Console.WriteLine();
            Console.WriteLine($"*** {this.name} SUBIU PARA O NIVEL {this.level}! ***");
            Console.WriteLine($"    Vida maxima +20, Ataque +5, Defesa +3%, {this.recursoNome} +10");
            Console.WriteLine();
        }

        /*
            EXERCICIO 5 - guarda um item na primeira vaga livre do inventario.
            Retorna false se as 5 vagas ja estiverem ocupadas.
        */
        public bool AdicionarItem(string item)
        {
            for (int i = 0; i < this.inventario.Length; i++)
            {
                if (this.inventario[i] == "")
                {
                    this.inventario[i] = item;
                    return true;   // guardou e ja sai do metodo
                }
            }

            return false;   // percorreu as 5 vagas e nenhuma estava livre
        }

        // EXERCICIO 5 - exibe todos os itens cadastrados.
        public void MostrarInventario()
        {
            Console.WriteLine("--- INVENTARIO ---");

            for (int i = 0; i < this.inventario.Length; i++)
            {
                if (this.inventario[i] == "")
                {
                    Console.WriteLine($"  {i + 1}. [vazio]");
                }
                else
                {
                    Console.WriteLine($"  {i + 1}. {this.inventario[i]}");
                }
            }
        }

        // Mostra o estado atual do personagem na tela.
        public void MostrarStatus()
        {
            Console.WriteLine($"{this.name} | Nivel {this.level} | Vida: {this.health}/{this.maxHealth} | {this.recursoNome}: {this.recurso}/{this.recursoMax} | Ataque: {this.damage} | Defesa: {this.defense}% | EXP: {this.exp}/{this.expParaSubir} | Moedas: {this.gold}");
        }
    }
/* Primeira subclasse herdando os valores base com o ":" de player */
public class Mago : Player
{
    public Mago(string name, int health, int damage, int defense, int level, int mana)
    /* colocando os valores base da classe Player*/
    : base(name, health, damage, defense, level) 
    {
        this.recurso = mana;
        this.recursoMax = mana;
        this.recursoNome = "Mana";
    }
}

public class Guerreiro : Player
{
    public Guerreiro(string name, int health, int damage, int defense, int level, int stamina)
    : base(name, health, damage, defense, level)
    {
        this.recurso = stamina;
        this.recursoMax = stamina;
        this.recursoNome = "Stamina";
    }
}

public class Arqueiro : Player
{
    public Arqueiro(string name, int health, int damage, int defense, int level, int stamina)
    : base(name, health, damage, defense, level)
    {
        this.recurso = stamina;
        this.recursoMax = stamina;
        this.recursoNome = "Stamina";
    }
}
