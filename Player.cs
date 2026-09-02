public class Player
{
    public string Name { get; private set; }
    public bool IsAttacker { get; set; }

    public Player(string name)
    {
        Name = name;
        IsAttacker = false; 
    }
}