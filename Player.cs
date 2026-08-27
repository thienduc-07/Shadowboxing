public class Player
{
    public string Name { get; private set; }
    public bool IsAttacker { get; set; }
    public int Combo { get; private set; }

    public Player(string name)
    {
        Name = name;
        Combo = 0;
        IsAttacker = false; // Mặc định vào game sẽ set sau
    }

    // Tăng combo khi đoán trúng
    public void AddCombo()
    {
        Combo++;
    }

    // Xoá combo khi đoán sai
    public void ResetCombo()
    {
        Combo = 0;
    }
}