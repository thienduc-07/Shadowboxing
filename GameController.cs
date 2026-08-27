using UnityEngine;

public class GameController : MonoBehaviour
{
    // Gọi bộ não logic đã viết ở trên
    private ShadowBoxingLogic gameLogic;

    // Dùng nullable (Direction?) để biết người chơi đã bấm phím trong lượt này chưa
    private Direction? p1Move = null;
    private Direction? p2Move = null;

    void Start()
    {
        // Khởi tạo ván game
        gameLogic = new ShadowBoxingLogic("Player 1 (WASD)", "Player 2 (Mũi tên)");
        
        Debug.Log("== TRẬN ĐẤU BẮT ĐẦU ==");
        Debug.Log($"Người tấn công trước: {gameLogic.CurrentAttacker.Name}");
    }

    void Update()
    {
        // Nếu có người thắng rồi thì dừng, không nhận phím nữa
        if (gameLogic.IsGameOver) return;

        // 1. Bắt phím của Player 1 (W, A, S, D)
        if (p1Move == null)
        {
            if (Input.GetKeyDown(KeyCode.W)) p1Move = Direction.Up;
            else if (Input.GetKeyDown(KeyCode.S)) p1Move = Direction.Down;
            else if (Input.GetKeyDown(KeyCode.A)) p1Move = Direction.Left;
            else if (Input.GetKeyDown(KeyCode.D)) p1Move = Direction.Right;
        }

        // 2. Bắt phím của Player 2 (Mũi tên)
        if (p2Move == null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) p2Move = Direction.Up;
            else if (Input.GetKeyDown(KeyCode.DownArrow)) p2Move = Direction.Down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) p2Move = Direction.Left;
            else if (Input.GetKeyDown(KeyCode.RightArrow)) p2Move = Direction.Right;
        }

        // 3. Khi cả 2 người đều đã chốt hướng, tiến hành tính toán kết quả lượt đó
        if (p1Move != null && p2Move != null)
        {
            ResolveTurn();
        }
    }

    private void ResolveTurn()
    {
        // Tách biệt rạch ròi: Ai đang làm Attacker thì lấy phím của người đó làm đòn tấn công
        Direction attackerMove = gameLogic.Player1.IsAttacker ? p1Move.Value : p2Move.Value;
        Direction defenderMove = gameLogic.Player1.IsAttacker ? p2Move.Value : p1Move.Value;

        Debug.Log($"[LƯỢT ĐÁNH] Tấn công chỉ: {attackerMove} --- Phòng thủ quay: {defenderMove}");

        // Quăng 2 cái hướng vào hàm Core Logic
        gameLogic.ProcessTurn(attackerMove, defenderMove);

        // Kiểm tra kết quả sau khi xử lý
        if (gameLogic.IsGameOver)
        {
            Debug.Log($"🏆 K.O!!! {gameLogic.Winner.Name} ĐÃ CHIẾN THẮNG!");
        }
        else
        {
            Debug.Log($"=> Lượt tiếp theo! Tấn công hiện tại: {gameLogic.CurrentAttacker.Name} | Combo: {gameLogic.CurrentAttacker.Combo}/3");
        }

        // Reset lại phím để chờ lượt chém gió tiếp theo
        p1Move = null;
        p2Move = null;
    }
}