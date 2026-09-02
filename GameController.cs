using UnityEngine;
using System.Collections;

public enum Direction
{
    Up, Down, Left, Right
}

public class GameController : MonoBehaviour
{
    private ShadowBoxingLogic gameLogic;

    private Direction? p1Move = null;
    private Direction? p2Move = null;
    
    // Khóa không cho nhận phím khi nhân vật đang diễn hoạt động tác
    private bool isAnimating = false;

    // Biến chứa hình ảnh của 2 nhân vật (Kéo thả Player1 và Player2 vào đây trên Unity)
    public PlayerVisual p1Visual;
    public PlayerVisual p2Visual;

    void Start()
    {
        gameLogic = new ShadowBoxingLogic("Player 1", "Player 2");
        Debug.Log("== TRẬN ĐẤU BẮT ĐẦU ==");
    }

    void Update()
    {
        // Khóa update nếu game over hoặc đang bận tua lại animation
        if (gameLogic.IsGameOver || isAnimating) return;

        // Bắt phím Player 1 (WASD)
        if (p1Move == null)
        {
            if (Input.GetKeyDown(KeyCode.W)) p1Move = Direction.Up;
            else if (Input.GetKeyDown(KeyCode.S)) p1Move = Direction.Down;
            else if (Input.GetKeyDown(KeyCode.A)) p1Move = Direction.Left;
            else if (Input.GetKeyDown(KeyCode.D)) p1Move = Direction.Right;
        }

        // Bắt phím Player 2 (Mũi tên)
        if (p2Move == null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) p2Move = Direction.Up;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) p2Move = Direction.Down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) p2Move = Direction.Left;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) p2Move = Direction.Right;
        }

        // Khi cả 2 đã chốt hướng
        if (p1Move != null && p2Move != null)
        {
            Direction attackerMove = gameLogic.Player1.IsAttacker ? p1Move.Value : p2Move.Value;
            Direction defenderMove = gameLogic.Player1.IsAttacker ? p2Move.Value : p1Move.Value;
            
            // Bắt đầu chạy hiệu ứng nháy hình
            StartCoroutine(PlayTurnRoutine(attackerMove, defenderMove));
        }
    }

    // ==========================================
    // ĐOẠN CODE CỦA BẠN NẰM CHÍNH XÁC Ở ĐÂY NHÉ:
    // ==========================================
    private IEnumerator PlayTurnRoutine(Direction newAttackerMove, Direction newDefenderMove)
    {
        isAnimating = true;

        // 1. TUA LẠI CHUỖI ĐÃ TRÚNG
        foreach (Direction oldMove in gameLogic.ComboSequence)
        {
            // Trong quá khứ, 2 người đoán trúng nên hướng của cả 2 là giống nhau (oldMove)
            p1Visual.ShowAction(oldMove, gameLogic.Player1.IsAttacker);
            p2Visual.ShowAction(oldMove, gameLogic.Player2.IsAttacker);
            
            yield return new WaitForSeconds(0.4f); // Rút ngắn lại xíu cho dồn dập

            // Nhịp nghỉ
            p1Visual.ShowIdle();
            p2Visual.ShowIdle();
            
            yield return new WaitForSeconds(0.2f); 
        }

        // 2. DIỄN ĐÒN MỚI NHẤT
        // Ai đang tấn công thì lấy hướng tấn công, ai thủ thì lấy hướng thủ
        if (gameLogic.Player1.IsAttacker)
        {
            p1Visual.ShowAction(newAttackerMove, true);
            p2Visual.ShowAction(newDefenderMove, false);
        }
        else
        {
            p2Visual.ShowAction(newAttackerMove, true);
            p1Visual.ShowAction(newDefenderMove, false);
        }
        
        yield return new WaitForSeconds(0.8f);

        // 3. CHỐT KẾT QUẢ LOGIC
        gameLogic.ProcessTurn(newAttackerMove, newDefenderMove);

        if (gameLogic.IsGameOver)
        {
            Debug.Log($"🏆 K.O!!! {gameLogic.Winner.Name} ĐÃ CHIẾN THẮNG!");
        }
        else
        {
            if (newAttackerMove != newDefenderMove)
            {
                Debug.Log("TRẬT LẤT! Đổi lượt.");
            }
            
            // Trả cả 2 về trạng thái đứng yên
            p1Visual.ShowIdle();
            p2Visual.ShowIdle();
            
            p1Move = null;
            p2Move = null;
            isAnimating = false;
        }
    }
}