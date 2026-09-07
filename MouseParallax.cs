using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    [Header("Tùy chỉnh Parallax")]
    public float moveAmount = 30f;  // Biên độ di chuyển (số càng to đi càng xa)
    public float moveSpeed = 5f;    // Độ mượt (số càng to đi càng bám sát chuột)

    private Vector3 startPos;

    void Start()
    {
        // Lưu lại vị trí ban đầu của Background
        startPos = transform.position;
    }

    void Update()
    {
        // Tính toán vị trí chuột quy đổi ra khoảng từ -0.5 đến 0.5
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        // Tạo hướng di chuyển. 
        // Đang để dấu trừ (-) để background đi ngược hướng chuột (tạo cảm giác 3D thật hơn).
        Vector3 targetOffset = new Vector3(-mouseX * moveAmount, -mouseY * moveAmount, 0);

        // Nội suy (Lerp) để hình ảnh di chuyển mượt mà, không bị giật
        transform.position = Vector3.Lerp(transform.position, startPos + targetOffset, Time.deltaTime * moveSpeed);
    }
}