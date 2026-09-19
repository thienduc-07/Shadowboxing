using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Giao diện các Tab (Panels)")]
    [SerializeField] private GameObject volumeContent;
    [SerializeField] private GameObject rulesContent;

    [Header("Nút bấm chuyển Tab (Buttons)")]
    [SerializeField] private Button btnVolumeTab;
    [SerializeField] private Button btnRulesTab;

    [Header("Hình ảnh nút (Phong cách Pixel)")]
    [SerializeField] private Sprite activeTabSprite;   
    [SerializeField] private Sprite inactiveTabSprite; 

    void Start()
    {
        btnVolumeTab.onClick.AddListener(ShowVolumeTab);
        btnRulesTab.onClick.AddListener(ShowRulesTab);
        ShowVolumeTab();
    }

    public void ShowVolumeTab()
    {
        volumeContent.SetActive(true);
        rulesContent.SetActive(false);
        UpdateButtonVisuals(btnVolumeTab, btnRulesTab);
    }

    public void ShowRulesTab()
    {
        volumeContent.SetActive(false);
        rulesContent.SetActive(true);
        UpdateButtonVisuals(btnRulesTab, btnVolumeTab);
    }

    private void UpdateButtonVisuals(Button activeBtn, Button inactiveBtn)
    {
        if (activeTabSprite != null && inactiveTabSprite != null)
        {
            activeBtn.GetComponent<Image>().sprite = activeTabSprite;
            inactiveBtn.GetComponent<Image>().sprite = inactiveTabSprite;
        }
    }
}