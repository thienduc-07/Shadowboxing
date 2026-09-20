using UnityEngine;
using TMPro;

public class FloatingIndicator : MonoBehaviour
{
    [Header("Thành phần UI")]
    [SerializeField] private SpriteRenderer arrowSprite; 
    [SerializeField] private TextMeshPro textName; // Chữ hiển thị tên (P1, P2...)

    [Header("Màu sắc trạng thái")]
    [SerializeField] private Color attackColor = new Color(1f, 0.2f, 0.2f); // Đỏ
    [SerializeField] private Color defendColor = Color.white; // Trắng (Base)

    [Header("Hiệu ứng trôi nổi (Floating)")]
    [SerializeField] private float floatSpeed = 4f;       
    [SerializeField] private float floatAmplitude = 0.2f; 

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }

    // Nhận thêm biến playerName để GameController truyền chữ "P1", "P2" vào
    public void UpdateState(bool isAttacker, string playerName)
    {
        Color targetColor = isAttacker ? attackColor : defendColor;

        if (arrowSprite != null) arrowSprite.color = targetColor;
        
        if (textName != null)
        {
            textName.text = playerName;
            textName.color = targetColor;
        }
    }
}