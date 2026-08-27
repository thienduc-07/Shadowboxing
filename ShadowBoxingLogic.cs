public class ShadowBoxingLogic
{
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    
    public Player CurrentAttacker { get; private set; }
    public Player CurrentDefender { get; private set; }
    
    public bool IsGameOver { get; private set; }
    public Player Winner { get; private set; }

    private const int WINNING_COMBO = 3;

    public ShadowBoxingLogic(string p1Name, string p2Name)
    {
        Player1 = new Player(p1Name);
        Player2 = new Player(p2Name);
        
        // Random hoặc set cứng người đi trước. Ở đây cho Player 1 đánh trước.
        Player1.IsAttacker = true;
        CurrentAttacker = Player1;
        CurrentDefender = Player2;
        IsGameOver = false;
    }

    // Hàm nhận input của cả 2 và xử lý kết quả lượt chơi
    public void ProcessTurn(Direction attackerMove, Direction defenderMove)
    {
        if (IsGameOver) return; // Game kết thúc rồi thì không xử lý nữa

        if (attackerMove == defenderMove)
        {
            // Đoán trúng -> Tăng combo
            CurrentAttacker.AddCombo();
            
            if (CurrentAttacker.Combo >= WINNING_COMBO)
            {
                IsGameOver = true;
                Winner = CurrentAttacker;
            }
        }
        else
        {
            // Lệch hướng -> Reset combo và đổi vai trò
            CurrentAttacker.ResetCombo();
            SwapRoles();
        }
    }

    private void SwapRoles()
    {
        // Đổi trạng thái cờ
        CurrentAttacker.IsAttacker = false;
        CurrentDefender.IsAttacker = true;

        // Hoán đổi vị trí con trỏ (reference)
        Player temp = CurrentAttacker;
        CurrentAttacker = CurrentDefender;
        CurrentDefender = temp;
    }
}