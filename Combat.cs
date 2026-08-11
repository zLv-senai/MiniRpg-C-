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