using UnityEngine;
using UnityEngine.UI;

public class setting : MonoBehaviour
{
    public static setting Instance;

    [Header("Panels")]
    [Tooltip("Panel หน้าต่างตั้งค่า")]
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public AudioSource bgmSource; // อ้างอิง AudioSource เพลง BGM (ถ้ามี)
    public AudioSource sfxSource; // อ้างอิง AudioSource เสียงเอฟเฟกต์ SFX (ถ้ามี)

    [Header("Vibration / Haptic (Mobile)")]
    public Toggle vibrationToggle;
    public static bool isVibrationEnabled = true;

    [Header("UI Buttons")]
    public Button openButton;
    public Button closeButton;
    public Button resetDataButton;

    private const string BGM_KEY = "Setting_BGM_Volume";
    private const string SFX_KEY = "Setting_SFX_Volume";
    private const string VIB_KEY = "Setting_Vibration";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSettings();
    }

    private void Start()
    {
        // ผูก Listener ให้กับ UI อัตโนมัติ (ถ้ามีต่อไว้ใน Inspector)
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (vibrationToggle != null)
        {
            vibrationToggle.onValueChanged.AddListener(SetVibration);
        }

        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenSettings);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }

        if (resetDataButton != null)
        {
            resetDataButton.onClick.AddListener(ResetAllGameData);
        }
    }

    // --- Panel Controls ---
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // --- Audio Controls ---
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }

        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }

        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();
    }

    // --- Vibration Controls (Mobile) ---
    public void SetVibration(bool isEnabled)
    {
        isVibrationEnabled = isEnabled;
        PlayerPrefs.SetInt(VIB_KEY, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void TriggerVibrate()
    {
        if (isVibrationEnabled)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }

    // --- Save & Load ---
    private void LoadSettings()
    {
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        isVibrationEnabled = PlayerPrefs.GetInt(VIB_KEY, 1) == 1;

        if (bgmSlider != null) bgmSlider.value = bgmVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;
        if (vibrationToggle != null) vibrationToggle.isOn = isVibrationEnabled;

        if (bgmSource != null) bgmSource.volume = bgmVol;
        if (sfxSource != null) sfxSource.volume = sfxVol;
    }

    // --- Reset All Data ---
    public void ResetAllGameData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // รีเซ็ตค่าใน GameManager ถ้ามี
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pickles = 0;
            GameManager.Instance.rebirthCount = 0;
            GameManager.Instance.UpdateUI();
        }

        // โหลดค่า Settings กลับมาเป็นค่า Default
        LoadSettings();

        Debug.Log("[Setting] รีเซ็ตข้อมูลเกมทั้งหมดเรียบร้อยแล้ว!");
    }
}
