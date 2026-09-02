using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Trạng thái mặc định")]
    public Sprite idleSprite;

    [Header("Hình Tấn Công (Tay chỉ)")]
    public Sprite attackUp;
    public Sprite attackDown;
    public Sprite attackLeft;
    public Sprite attackRight;

    [Header("Hình Phòng Thủ (Quay đầu)")]
    public Sprite defendUp;
    public Sprite defendDown;
    public Sprite defendLeft;
    public Sprite defendRight;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Lấy component SpriteRenderer gắn trên chính GameObject này
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Hàm gọi để đưa nhân vật về thế đứng yên
    public void ShowIdle()
    {
        spriteRenderer.sprite = idleSprite;
    }

    // Hàm gọi để chốt hình dựa vào Hướng (dir) và Vai trò (isAttacking)
    public void ShowAction(Direction dir, bool isAttacking)
    {
        if (isAttacking)
        {
            switch (dir)
            {
                case Direction.Up: spriteRenderer.sprite = attackUp; break;
                case Direction.Down: spriteRenderer.sprite = attackDown; break;
                case Direction.Left: spriteRenderer.sprite = attackLeft; break;
                case Direction.Right: spriteRenderer.sprite = attackRight; break;
            }
        }
        else // Nếu đang phòng thủ
        {
            switch (dir)
            {
                case Direction.Up: spriteRenderer.sprite = defendUp; break;
                case Direction.Down: spriteRenderer.sprite = defendDown; break;
                case Direction.Left: spriteRenderer.sprite = defendLeft; break;
                case Direction.Right: spriteRenderer.sprite = defendRight; break;
            }
        }
    }
}