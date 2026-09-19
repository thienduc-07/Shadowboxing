using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    [Header("Thông số nhịp thở")]
    [SerializeField] private float speed = 3.5f;        
    [SerializeField] private float amountY = 0.015f;    
    [SerializeField] private float amountX = 0.005f;    

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float breathY = Mathf.Sin(Time.time * speed) * amountY;
        float breathX = Mathf.Cos(Time.time * speed) * amountX; 
        
        transform.localScale = new Vector3(startScale.x - breathX, startScale.y + breathY, startScale.z);
    }
}