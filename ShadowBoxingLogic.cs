using System.Collections.Generic;

public class ShadowBoxingLogic
{
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    
    public Player CurrentAttacker { get; private set; }
    public Player CurrentDefender { get; private set; }
    
    public bool IsGameOver { get; private set; }
    public Player Winner { get; private set; }

    // Trừu tượng hóa việc lấy chuỗi combo ra ngoài cho GameController
    public IReadOnlyList<Direction> ComboSequence => CurrentAttacker.ComboSequence;

    private const int WINNING_COMBO = 3;

    public ShadowBoxingLogic(string p1Name, string p2Name)
    {
        Player1 = new Player(p1Name);
        Player2 = new Player(p2Name);
        
        Player1.IsAttacker = true;
        CurrentAttacker = Player1;
        CurrentDefender = Player2;
    }

    public void ProcessTurn(Direction attackerMove, Direction defenderMove)
    {
        if (IsGameOver) return;

        if (attackerMove == defenderMove)
        {
            CurrentAttacker.AddCombo(attackerMove);
            
            if (CurrentAttacker.ComboCount >= WINNING_COMBO)
            {
                IsGameOver = true;
                Winner = CurrentAttacker;
            }
        }
        else
        {
            CurrentAttacker.ResetCombo();
            SwapRoles();
        }
    }

    private void SwapRoles()
    {
        CurrentAttacker.IsAttacker = false;
        CurrentDefender.IsAttacker = true;

        (CurrentAttacker, CurrentDefender) = (CurrentDefender, CurrentAttacker); // Cú pháp Tuple hoán đổi của C#
    }
}