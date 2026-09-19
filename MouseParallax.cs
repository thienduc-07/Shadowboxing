using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    [Header("Tùy chỉnh Parallax")]
    [SerializeField] private float moveAmount = 30f;  
    [SerializeField] private float moveSpeed = 5f;    

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        Vector3 targetOffset = new Vector3(-mouseX * moveAmount, -mouseY * moveAmount, 0);

        transform.position = Vector3.Lerp(transform.position, startPos + targetOffset, Time.deltaTime * moveSpeed);
    }
}