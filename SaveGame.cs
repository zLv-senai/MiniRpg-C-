/*
    SaveGame.cs
    -----------
    Salva e carrega o progresso do jogador em arquivo texto.
    Cobre o desafio extra "Salvar progresso em arquivo texto" da lista.

    Cada personagem vira um arquivo dentro da pasta "saves":
        saves/Lv.txt
        saves/Aragorn.txt

    Formato do arquivo - uma informacao por linha, no estilo chave=valor:

        nome=Lv
        classe=Guerreiro
        nivel=3
        vida=95
        ...

    Esse formato foi escolhido porque da para ABRIR NO BLOCO DE NOTAS e
    entender o que esta salvo, o que ajuda a achar erro.
*/

/*
    Pacote com tudo que compoe uma partida salva.
    Existe porque carregar um save devolve VARIAS coisas ao mesmo tempo
    (o heroi, as missoes e os contadores) e um metodo so pode ter um retorno.
*/
public class DadosDoJogo
{
    public Player heroi {get; set;}
    public List<Missao> missoes {get; set;}
    public int inimigosDerrotados {get; set;}
    public int itensComprados {get; set;}

    public DadosDoJogo(Player heroi, List<Missao> missoes, int inimigosDerrotados, int itensComprados)
    {
        this.heroi = heroi;
        this.missoes = missoes;
        this.inimigosDerrotados = inimigosDerrotados;
        this.itensComprados = itensComprados;
    }
}

public class SaveGame
{
    private const string PASTA = "saves";

    // ---------------------------------------------------------------
    // SALVAR
    // ---------------------------------------------------------------

    public static void Salvar(Player heroi, List<Missao> missoes, int inimigosDerrotados, int itensComprados)
    {
        // Cria a pasta se ela nao existir. Se ja existir, nao faz nada
        // e nao da erro - por isso nao precisa de if antes.
        Directory.CreateDirectory(PASTA);

        List<string> linhas = new List<string>();

        linhas.Add("nome=" + heroi.name);
        linhas.Add("classe=" + IdentificarClasse(heroi));
        linhas.Add("nivel=" + heroi.level);
        linhas.Add("vida=" + heroi.health);
        linhas.Add("vidaMax=" + heroi.maxHealth);
        linhas.Add("dano=" + heroi.damage);
        linhas.Add("defesa=" + heroi.defense);
        linhas.Add("recurso=" + heroi.recurso);
        linhas.Add("recursoMax=" + heroi.recursoMax);
        linhas.Add("exp=" + heroi.exp);
        linhas.Add("gold=" + heroi.gold);
        linhas.Add("inimigosDerrotados=" + inimigosDerrotados);
        linhas.Add("itensComprados=" + itensComprados);

        /*
            O inventario tem 5 vagas e vira uma linha so, com as vagas
            separadas por barra vertical. Uma vaga vazia fica sem nada
            entre as barras:
                inventario=Espada|Pocao|||
        */
        string inventarioTexto = "";
        for (int i = 0; i < heroi.inventario.Length; i++)
        {
            inventarioTexto = inventarioTexto + heroi.inventario[i];

            if (i < heroi.inventario.Length - 1)
            {
                inventarioTexto = inventarioTexto + "|";
            }
        }
        linhas.Add("inventario=" + inventarioTexto);

        /*
            Do equipamento salvamos apenas o NOME. Os bonus vivem no catalogo
            da loja, entao gravar tudo de novo seria duplicar informacao - e
            se um dia a Espada mudasse de bonus, os saves antigos ficariam
            com o valor velho.
        */
        string nomeArma = "";
        if (heroi.arma != null)
        {
            nomeArma = heroi.arma.nome;
        }
        linhas.Add("arma=" + nomeArma);

        string nomeArmadura = "";
        if (heroi.armadura != null)
        {
            nomeArmadura = heroi.armadura.nome;
        }
        linhas.Add("armadura=" + nomeArmadura);

        /*
            Das missoes so precisamos saber quais estao concluidas, porque
            a lista de missoes em si e sempre recriada igual pelo jogo.
            Salvamos os nomes das concluidas separados por barra.
        */
        string missoesTexto = "";
        for (int i = 0; i < missoes.Count; i++)
        {
            if (missoes[i].concluida)
            {
                if (missoesTexto != "")
                {
                    missoesTexto = missoesTexto + "|";
                }
                missoesTexto = missoesTexto + missoes[i].nome;
            }
        }
        linhas.Add("missoesConcluidas=" + missoesTexto);

        string caminho = Path.Combine(PASTA, LimparNome(heroi.name) + ".txt");

        // WriteAllLines SUBSTITUI o arquivo inteiro. Diferente do
        // AppendAllText do ranking, que acrescenta no fim.
        File.WriteAllLines(caminho, linhas);
    }

    // ---------------------------------------------------------------
    // LISTAR
    // ---------------------------------------------------------------

    // Devolve o nome de cada save existente (sem a extensao .txt).
    public static List<string> ListarSaves()
    {
        List<string> nomes = new List<string>();

        if (Directory.Exists(PASTA) == false)
        {
            return nomes;   // pasta nem existe: lista vazia
        }

        string[] arquivos = Directory.GetFiles(PASTA, "*.txt");

        for (int i = 0; i < arquivos.Length; i++)
        {
            nomes.Add(Path.GetFileNameWithoutExtension(arquivos[i]));
        }

        return nomes;
    }

    // ---------------------------------------------------------------
    // CARREGAR
    // ---------------------------------------------------------------

    /*
        O "?" em "DadosDoJogo?" avisa que este metodo PODE devolver nulo -
        quando o arquivo nao existe ou esta corrompido. Quem chama e obrigado
        a testar antes de usar. Em Python voce devolveria None e nada
        obrigaria a checar; aqui o compilador cobra.
    */
    public static DadosDoJogo? Carregar(string nomeSave)
    {
        string caminho = Path.Combine(PASTA, nomeSave + ".txt");

        if (File.Exists(caminho) == false)
        {
            return null;
        }

        string[] linhas = File.ReadAllLines(caminho);

        /*
            Dictionary guarda pares chave -> valor, igual ao dicionario do
            Python: dados["nivel"] devolve "3".
            Usar dicionario em vez de contar linhas deixa o save resistente
            a mudanca de ordem das linhas.
        */
        Dictionary<string, string> dados = new Dictionary<string, string>();

        for (int i = 0; i < linhas.Length; i++)
        {
            if (linhas[i] == "")
            {
                continue;
            }

            /*
                O 2 no Split limita a divisao a duas partes. Sem ele, um nome
                de item que tivesse "=" quebraria a leitura.
            */
            string[] partes = linhas[i].Split('=', 2);

            if (partes.Length != 2)
            {
                continue;
            }

            dados[partes[0]] = partes[1];
        }

        string nome = LerTexto(dados, "nome", "Heroi");
        string classe = LerTexto(dados, "classe", "Guerreiro");

        /*
            Recria o personagem da classe certa. Os numeros passados no
            construtor sao provisorios: as linhas seguintes sobrescrevem
            tudo com os valores salvos.
        */
        Player heroi;

        if (classe == "Mago")
        {
            heroi = new Mago(nome, 1, 1, 1, 1, 1);
            heroi.ataques = CatalogoAtaques.DoMago();
        }
        else if (classe == "Arqueiro")
        {
            heroi = new Arqueiro(nome, 1, 1, 1, 1, 1);
            heroi.ataques = CatalogoAtaques.DoArqueiro();
        }
        else
        {
            heroi = new Guerreiro(nome, 1, 1, 1, 1, 1);
            heroi.ataques = CatalogoAtaques.DoGuerreiro();
        }

        heroi.level = LerNumero(dados, "nivel", 1);
        heroi.maxHealth = LerNumero(dados, "vidaMax", 100);
        heroi.health = LerNumero(dados, "vida", heroi.maxHealth);
        heroi.damage = LerNumero(dados, "dano", 10);
        heroi.defense = LerNumero(dados, "defesa", 5);
        heroi.recursoMax = LerNumero(dados, "recursoMax", 40);
        heroi.recurso = LerNumero(dados, "recurso", heroi.recursoMax);
        heroi.exp = LerNumero(dados, "exp", 0);
        heroi.gold = LerNumero(dados, "gold", 0);

        // Inventario: desfaz a linha "Espada|Pocao|||" de volta nas 5 vagas.
        string inventarioTexto = LerTexto(dados, "inventario", "");
        string[] vagas = inventarioTexto.Split('|');

        for (int i = 0; i < heroi.inventario.Length; i++)
        {
            if (i < vagas.Length)
            {
                heroi.inventario[i] = vagas[i];
            }
            else
            {
                heroi.inventario[i] = "";
            }
        }

        // Equipamento: busca o item no catalogo pelo nome salvo.
        string nomeArmaSalva = LerTexto(dados, "arma", "");
        if (nomeArmaSalva != "")
        {
            heroi.arma = Loja.BuscarPorNome(nomeArmaSalva);
        }

        string nomeArmaduraSalva = LerTexto(dados, "armadura", "");
        if (nomeArmaduraSalva != "")
        {
            heroi.armadura = Loja.BuscarPorNome(nomeArmaduraSalva);
        }

        // Missoes: recria a lista padrao e remarca as que estavam concluidas.
        List<Missao> missoes = GerenciadorMissoes.CriarMissoes();
        string missoesTexto = LerTexto(dados, "missoesConcluidas", "");

        if (missoesTexto != "")
        {
            string[] concluidas = missoesTexto.Split('|');

            for (int i = 0; i < missoes.Count; i++)
            {
                for (int j = 0; j < concluidas.Length; j++)
                {
                    if (missoes[i].nome == concluidas[j])
                    {
                        missoes[i].concluida = true;
                    }
                }
            }
        }

        int inimigos = LerNumero(dados, "inimigosDerrotados", 0);
        int itens = LerNumero(dados, "itensComprados", 0);

        return new DadosDoJogo(heroi, missoes, inimigos, itens);
    }

    // ---------------------------------------------------------------
    // AUXILIARES
    // ---------------------------------------------------------------

    /*
        Descobre a classe do personagem em tempo de execucao.
        O operador "is" pergunta "este objeto e deste tipo?" - equivale ao
        isinstance(heroi, Mago) do Python.
    */
    public static string IdentificarClasse(Player heroi)
    {
        if (heroi is Mago)
        {
            return "Mago";
        }

        if (heroi is Arqueiro)
        {
            return "Arqueiro";
        }

        return "Guerreiro";
    }

    // Tira do nome os caracteres que o Windows nao aceita em nome de arquivo.
    public static string LimparNome(string nome)
    {
        string limpo = nome;
        char[] proibidos = Path.GetInvalidFileNameChars();

        for (int i = 0; i < proibidos.Length; i++)
        {
            limpo = limpo.Replace(proibidos[i].ToString(), "");
        }

        if (limpo == "")
        {
            limpo = "Heroi";
        }

        return limpo;
    }

    // Le um texto do dicionario, devolvendo um padrao se a chave nao existir.
    private static string LerTexto(Dictionary<string, string> dados, string chave, string padrao)
    {
        if (dados.ContainsKey(chave))
        {
            return dados[chave];
        }

        return padrao;
    }

    // Le um numero do dicionario, devolvendo um padrao se faltar ou for invalido.
    private static int LerNumero(Dictionary<string, string> dados, string chave, int padrao)
    {
        if (dados.ContainsKey(chave) == false)
        {
            return padrao;
        }

        int valor;
        if (int.TryParse(dados[chave], out valor) == false)
        {
            return padrao;
        }

        return valor;
    }
}
