using UnityEngine;

[System.Serializable]
public class DirectionalSprites
{
    // Để nguyên public cho class Data Serializable để Unity Inspector đọc được
    public Sprite up, down, left, right;

    public Sprite GetSprite(Direction dir) => dir switch
    {
        Direction.Up => up,
        Direction.Down => down,
        Direction.Left => left,
        Direction.Right => right,
        _ => null
    };
}

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    
    [Header("Bộ hình ảnh Tấn Công & Phòng Thủ")]
    [SerializeField] private DirectionalSprites attackSprites;
    [SerializeField] private DirectionalSprites defendSprites;

    // ---> THÊM DÒNG NÀY: Khai báo Indicator
    [Header("Hiệu ứng trên đầu")]
    [SerializeField] private FloatingIndicator indicator; 

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowIdle()
    {
        spriteRenderer.sprite = idleSprite;
    }

    public void ShowAction(Direction dir, bool isAttacking)
    {
        DirectionalSprites currentSet = isAttacking ? attackSprites : defendSprites;
        spriteRenderer.sprite = currentSet.GetSprite(dir);
    }


    public void SetIndicatorState(bool isAttacker, string playerName)
    {
        if (indicator != null)
        {
            indicator.UpdateState(isAttacker, playerName);
        }
    }
    public void HideIndicator()
    {
        if (indicator != null)
        {
            indicator.gameObject.SetActive(false); // Ẩn cái mũi tên đi
        }
    }
    
}
