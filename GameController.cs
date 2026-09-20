using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private bool isAIMode;
    private ShadowBoxingLogic gameLogic;

    // OOP: Sử dụng Interface (Tính đa hình) thay vì code cứng phím bấm
    private IInputProvider p1InputProvider;
    private IInputProvider p2InputProvider;

    private Direction? p1Move = null;
    private Direction? p2Move = null;
    private bool isAnimating = false;

    private static bool hasBeenLoadedBefore = false;

    // OOP: Tính đóng gói (Encapsulation). Dùng [SerializeField] private thay cho public
    [Header("Nhân Vật")]
    [SerializeField] private PlayerVisual p1Visual;
    [SerializeField] private PlayerVisual p2Visual;

    [Header("Giao Diện (UI)")]
    [SerializeField] private TextMeshProUGUI txtAttacker;
    [SerializeField] private TextMeshProUGUI txtCombo;
    [SerializeField] private TextMeshProUGUI txtWin;
    [SerializeField] private GameObject btnReplay;
    [SerializeField] private GameObject btnHome;
    [SerializeField] private GameObject btnIngameHome;
    [SerializeField] private GameObject btnSettings;

    [Header("Giao Diện Mute")]
    [SerializeField] private GameObject btnMute;
    [SerializeField] private Image iconSpeakerImage;
    [SerializeField] private Image iconCrossImage;
    [SerializeField] private Color colorSpeakerOn = Color.white;
    [SerializeField] private Color colorSpeakerMute = Color.gray;

    private bool isMuted = false;

    [Header("Âm Thanh")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip comboSound;
    [SerializeField] private AudioClip koSound;

    [Header("Cài Đặt (Settings)")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string LAST_BGM_VOLUME_KEY = "LastBGMVolume";
    private const string LAST_SFX_VOLUME_KEY = "LastSFXVolume";
    private const string MUTE_KEY = "IsMuted";

    private const float DEFAULT_BGM_VOLUME = 0.5f;
    private const float DEFAULT_SFX_VOLUME = 0.5f;
    private const float VOLUME_CURVE = 2f;

    private float targetBGMVolume = 0.5f;
    private float targetSFXVolume = 0.5f;

    void Start()
    {
        isAIMode = PlayerPrefs.GetInt("IsAIMode", 0) == 1;

        string p2Name = isAIMode ? "AI BOT" : "Player2";
        gameLogic = new ShadowBoxingLogic("Player1", p2Name);

        // Khởi tạo Input dựa trên chế độ chơi (OOP: Strategy Pattern)
        p1InputProvider = new WASDInput();
        p2InputProvider = isAIMode ? (IInputProvider)new AIInput() : new ArrowKeyInput();

        if (txtWin != null) txtWin.text = "";
        if (btnReplay != null) btnReplay.SetActive(false);
        if (btnHome != null) btnHome.SetActive(false);
        if (btnIngameHome != null) btnIngameHome.SetActive(true);
        if (btnMute != null) btnMute.SetActive(true);
        if (btnSettings != null) btnSettings.SetActive(true);

        float savedBGM = Mathf.Clamp01(PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_BGM_VOLUME));
        float savedSFX = Mathf.Clamp01(PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME));

        if (!hasBeenLoadedBefore)
        {
            isMuted = false;
            PlayerPrefs.SetInt(MUTE_KEY, 0);
            hasBeenLoadedBefore = true;
        }
        else
        {
            isMuted = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        }

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(savedBGM);
            bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(savedSFX);
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        UpdateTargetVolumes();

        if (bgmSource != null)
        {
            bgmSource.volume = GetAudioVolume(targetBGMVolume);
            if (!bgmSource.isPlaying && bgmSource.clip != null)
            {
                bgmSource.Play();
            }
        }

        if (sfxSource != null)
        {
            sfxSource.volume = GetAudioVolume(targetSFXVolume);
        }

        UpdateMuteUI();
        UpdateUI();

        if (p1Visual != null) p1Visual.ShowIdle();
        if (p2Visual != null) p2Visual.ShowIdle();
    }

    void Update()
    {
        if (bgmSource != null)
        {
            float targetActualBGM = isMuted ? 0f : GetAudioVolume(targetBGMVolume);
            bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, targetActualBGM, Time.unscaledDeltaTime * 12f);
        }

        if (sfxSource != null)
        {
            float targetActualSFX = isMuted ? 0f : GetAudioVolume(targetSFXVolume);
            sfxSource.volume = Mathf.MoveTowards(sfxSource.volume, targetActualSFX, Time.unscaledDeltaTime * 12f);
        }

        if (gameLogic.IsGameOver || isAnimating)
            return;

        // OOP: Gọi hàm GetInput từ Interface, code cực kỳ gọn gàng
        if (p1Move == null)
        {
            p1Move = p1InputProvider.GetInput(gameLogic.ComboSequence);
        }

        if (p2Move == null)
        {
            // Nếu là AI, nó chỉ lấy input khi Player 1 đã bấm
            if (isAIMode && p1Move != null)
            {
                p2Move = p2InputProvider.GetInput(gameLogic.ComboSequence);
            }
            else if (!isAIMode)
            {
                p2Move = p2InputProvider.GetInput(gameLogic.ComboSequence);
            }
        }

        if (p1Move != null && p2Move != null)
        {
            Direction attackerMove = gameLogic.Player1.IsAttacker ? p1Move.Value : p2Move.Value;
            Direction defenderMove = gameLogic.Player1.IsAttacker ? p2Move.Value : p1Move.Value;
            StartCoroutine(PlayTurnRoutine(attackerMove, defenderMove));
        }
    }

    private IEnumerator PlayTurnRoutine(Direction newAttackerMove, Direction newDefenderMove)
    {
        isAnimating = true;

        foreach (Direction oldMove in gameLogic.ComboSequence)
        {
            p1Visual.ShowAction(oldMove, gameLogic.Player1.IsAttacker);
            p2Visual.ShowAction(oldMove, gameLogic.Player2.IsAttacker);
            PlaySFX(hitSound);
            yield return new WaitForSeconds(0.4f);

            p1Visual.ShowIdle();
            p2Visual.ShowIdle();
            yield return new WaitForSeconds(0.2f);
        }

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

        PlaySFX(hitSound);

        if (newAttackerMove == newDefenderMove)
        {
            PlaySFX(comboSound);
            StartCoroutine(ShakeCamera(0.2f, 0.15f, 0.5f));
            StartCoroutine(ComboPopEffect());
        }

        yield return new WaitForSeconds(0.8f);

        gameLogic.ProcessTurn(newAttackerMove, newDefenderMove);
        UpdateUI();

        if (gameLogic.IsGameOver)
        {
            StartCoroutine(GodTierKOEffect());
        }
        else
        {
            p1Visual.ShowIdle();
            p2Visual.ShowIdle();
            p1Move = null;
            p2Move = null;
            isAnimating = false;
        }
    }

    private void UpdateUI()
    {
        if (txtAttacker != null)
        {
            txtAttacker.text = "Turn:\n<color=#FFD700>" + gameLogic.CurrentAttacker.Name + "</color>";
        }

        if (txtCombo != null)
        {
            txtCombo.text = "Combo: " + gameLogic.ComboSequence.Count + "/3";
        }

        // Truyền thẳng tên rút gọn vào đây. Nếu đang chơi với máy thì báo là BOT.
        p1Visual.SetIndicatorState(gameLogic.Player1.IsAttacker, "P1");
        
        string p2Label = isAIMode ? "AI" : "P2";
        p2Visual.SetIndicatorState(gameLogic.Player2.IsAttacker, p2Label);
    }
    private IEnumerator ComboPopEffect()
    {
        if (txtCombo == null) yield break;

        Vector3 originalPos = txtCombo.transform.position;
        txtCombo.color = Color.red;

        for (int i = 0; i < 15; i++)
        {
            txtCombo.transform.position = originalPos + new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 0);
            yield return new WaitForSeconds(0.02f);
        }

        txtCombo.transform.position = originalPos;
        txtCombo.color = new Color(1f, 0.6f, 0f);
    }

    private IEnumerator GodTierKOEffect()
    {
        if (btnIngameHome != null) btnIngameHome.SetActive(false);
        if (btnMute != null) btnMute.SetActive(false);
        if (btnSettings != null) btnSettings.SetActive(false);

        if (bgmSource != null) bgmSource.Stop();
        PlaySFX(koSound);

        Time.timeScale = 0.05f;
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        float originalZoom = mainCam.orthographicSize;
        mainCam.orthographicSize = originalZoom * 0.6f;

        GameObject overlayObj = new GameObject("BlackOverlay");
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null) overlayObj.transform.SetParent(canvas.transform, false);
        overlayObj.transform.SetAsFirstSibling();

        Image bgImg = overlayObj.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.85f);
        RectTransform rect = overlayObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        PlayerVisual loserVisual = (gameLogic.Winner.Name == "Player1") ? p2Visual : p1Visual;

        StartCoroutine(ShakeCamera(0.3f, 1.2f, 5f));

        for (int i = 0; i < 5; i++)
        {
            bgImg.color = (i % 2 == 0) ? Color.red : Color.white;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        bgImg.color = new Color(0, 0, 0, 0.9f);

        float throwTime = 0;
        Vector3 throwDirection = new Vector3(Random.Range(-10f, 10f), 15f, 0);

        while (throwTime < 1.5f)
        {
            loserVisual.transform.position += throwDirection * Time.unscaledDeltaTime;
            loserVisual.transform.Rotate(0, 0, 1000f * Time.unscaledDeltaTime);
            throwTime += Time.unscaledDeltaTime;
            yield return null;
        }

        txtWin.text = "K.O !!";
        txtWin.color = Color.red;
        txtWin.transform.localScale = new Vector3(25f, 25f, 1f);
        txtWin.transform.localRotation = Quaternion.Euler(0, 0, 15f);

        float t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 10f;
            txtWin.transform.localScale = Vector3.Lerp(new Vector3(25f, 25f, 1f), Vector3.one, t);
            txtWin.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 15f), Quaternion.identity, t);
            yield return null;
        }

        StartCoroutine(ShakeCamera(0.3f, 0.6f, 2f));
        yield return new WaitForSecondsRealtime(1.0f);

        Time.timeScale = 1f;
        mainCam.orthographicSize = originalZoom;

        float fade = 1f;
        while (fade > 0.75f)
        {
            fade -= Time.deltaTime * 2f;
            bgImg.color = new Color(0, 0, 0, fade);
            yield return null;
        }

        txtWin.text = "<size=150%>K.O !!</size>\n<color=#FFD700>" + gameLogic.Winner.Name + " WINNN!!!</color>";

        if (btnReplay != null) btnReplay.SetActive(true);
        if (btnHome != null) btnHome.SetActive(true);
    }

    private IEnumerator ShakeCamera(float duration, float posMag, float rotMag)
    {
        Transform camT = Camera.main.transform;
        Vector3 origPos = camT.position;
        Quaternion origRot = camT.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * posMag;
            float y = Random.Range(-1f, 1f) * posMag;
            float zRot = Random.Range(-1f, 1f) * rotMag;

            camT.position = new Vector3(origPos.x + x, origPos.y + y, origPos.z);
            camT.rotation = Quaternion.Euler(origRot.eulerAngles.x, origRot.eulerAngles.y, zRot);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        camT.position = origPos;
        camT.rotation = origRot;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private float GetAudioVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp01(sliderValue);
        return Mathf.Pow(sliderValue, VOLUME_CURVE);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null || isMuted)
            return;

        sfxSource.PlayOneShot(clip);
    }

    private void UpdateTargetVolumes()
    {
        targetBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_BGM_VOLUME);
        targetSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (!isMuted) 
        {
            float currentBGM = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_BGM_VOLUME);
            float currentSFX = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);

            if (currentBGM <= 0.001f)
            {
                currentBGM = DEFAULT_BGM_VOLUME; 
                PlayerPrefs.SetFloat(BGM_VOLUME_KEY, currentBGM);
                if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(currentBGM);
            }

            if (currentSFX <= 0.001f)
            {
                currentSFX = DEFAULT_SFX_VOLUME; 
                PlayerPrefs.SetFloat(SFX_VOLUME_KEY, currentSFX);
                if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(currentSFX);
            }
        }

        PlayerPrefs.SetInt(MUTE_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateTargetVolumes();
        UpdateMuteUI();
    }
    private void UpdateMuteUI()
    {
        if (iconSpeakerImage != null)
        {
            iconSpeakerImage.color = isMuted ? colorSpeakerMute : colorSpeakerOn;
        }

        if (iconCrossImage != null)
        {
            iconCrossImage.gameObject.SetActive(isMuted);
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (btnMute != null) btnMute.SetActive(false);
        if (btnSettings != null) btnSettings.SetActive(false);
        if (btnIngameHome != null) btnIngameHome.SetActive(false);

        Time.timeScale = 0f;

        if (isMuted)
        {
            if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(0f);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            float savedBGM = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_BGM_VOLUME);
            float savedSFX = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);

            if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(savedBGM);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(savedSFX);
        }

        UpdateTargetVolumes();
        UpdateMuteUI();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (btnMute != null) btnMute.SetActive(true);
        if (btnSettings != null) btnSettings.SetActive(true);
        if (btnIngameHome != null) btnIngameHome.SetActive(true);

        Time.timeScale = 1f;

        if (bgmSlider != null && sfxSlider != null && bgmSlider.value <= 0.001f && sfxSlider.value <= 0.001f)
        {
            isMuted = true;
        }
        else
        {
            isMuted = false;
        }

        PlayerPrefs.SetInt(MUTE_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateTargetVolumes();
        UpdateMuteUI();
    }

    public void OnBGMVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);

        if (value > 0.001f && isMuted)
        {
            isMuted = false;
            PlayerPrefs.SetInt(MUTE_KEY, 0);
        }

        PlayerPrefs.Save();
        UpdateTargetVolumes();
    }

    public void OnSFXVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);

        if (value > 0.001f && isMuted)
        {
            isMuted = false;
            PlayerPrefs.SetInt(MUTE_KEY, 0);
        }

        PlayerPrefs.Save();
        UpdateTargetVolumes();
    }
}