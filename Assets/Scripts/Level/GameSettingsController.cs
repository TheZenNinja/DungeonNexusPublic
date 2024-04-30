using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Player;
using Skills.Player;

public class GameSettingsController : MonoBehaviour
{
    private const string STATE_PAUSE = "pause";
    private const string STATE_GAMEPLAY = "gameplay";
    private const string STATE_SKILL_SELECT = "skill";

    [Header("Vars")]
    [SerializeField] string currentMenu;
    [Header("Sliders")]
    [SerializeField] Slider mouseSlider;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [Header("References")]
    [SerializeField] InputActionReference escapeKey;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject UI;
    [Header("Player Refs")]
    [SerializeField] PlayerCamera playerCam;
    [SerializeField] PlayerEntity playerEntity;

    void Start()
    {
        Application.targetFrameRate = 60;

        LoadSettings();

        InitializeSliders();

        currentMenu = STATE_GAMEPLAY;
        UI.SetActive(false);

        escapeKey.action.performed += _ => HitEscape();

        mouseSlider.onValueChanged.AddListener((float v) => playerCam.SetSensitivity(v));

        masterSlider.onValueChanged.AddListener((float v) => AdjustVolume(v, "MasterVolume"));
        sfxSlider.onValueChanged.AddListener((float v) => AdjustVolume(v, "SFXVolume"));
        musicSlider.onValueChanged.AddListener((float v) => AdjustVolume(v, "MusicVolume"));
    }
    void InitializeSliders()
    {
        float v;
        audioMixer.GetFloat("MasterVolume", out v);
        masterSlider.value = ConvertFromDB(v);
        audioMixer.GetFloat("SFXVolume", out v);
        sfxSlider.value = ConvertFromDB(v);
        audioMixer.GetFloat("MusicVolume", out v);
        musicSlider.value = ConvertFromDB(v);

        mouseSlider.value = playerCam.GetSensitivity();
    }

    public void OpenSkillMenu() => currentMenu = STATE_SKILL_SELECT;
    public void CloseSkillMenu()
    {
        currentMenu = STATE_GAMEPLAY;
        playerEntity.SetLockAllInput(false);
    }

    public void HitEscape()
    {
        switch (currentMenu)
        {
            case STATE_PAUSE: //Close Pause Menu
                playerEntity.SetLockAllInput(false);
                UI.SetActive(false);
                currentMenu = STATE_GAMEPLAY;
                SaveSettings();
                break;
            case STATE_GAMEPLAY: //Open Pause Menu
                playerEntity.SetLockAllInput(true);
                UI.SetActive(true);
                currentMenu = STATE_PAUSE;
                break;
            case STATE_SKILL_SELECT:
                FindObjectOfType<SkillSelectionMenu>().CloseUI();
                currentMenu = STATE_GAMEPLAY;
                playerEntity.SetLockAllInput(false);
                break;
            default:
                Debug.LogError($"Menu state \"{currentMenu}\" not handled");
                break;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("LookSpeed", playerCam.GetSensitivity());

        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        PlayerPrefs.Save();
    }
    public void LoadSettings()
    {
        float master = TryGetPlayerPrefKey("MasterVolume", .8f);
        float sfx = TryGetPlayerPrefKey("SFXVolume", 1);
        float music = TryGetPlayerPrefKey("MusicVolume", 1);

        AdjustVolume(master, "MasterVolume");
        AdjustVolume(sfx, "SFXVolume");
        AdjustVolume(music, "MusicVolume");

        float lookSpeed = PlayerPrefs.GetFloat("LookSpeed", 5);
        playerCam.SetSensitivity(lookSpeed);
    }
    [Button]
    public void ClearSettings()
    {
        PlayerPrefs.DeleteKey("LookSpeed");

        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
        PlayerPrefs.DeleteKey("MusicVolume");

        PlayerPrefs.Save();
    }

    public void AdjustVolume(float value, string paramName)
    {
        float adjustedValue = ConvertToDB(value);
        audioMixer.SetFloat(paramName, adjustedValue);
    }

    float TryGetPlayerPrefKey(string key, float defaultValue)
    {
        float output;

        if (PlayerPrefs.HasKey(key))
            output = PlayerPrefs.GetFloat(key);
        else
        {
            output = defaultValue;
            PlayerPrefs.SetFloat(key, output);
            PlayerPrefs.Save();
        }

        return output;

    }

    // I hate decibels 
    // https://forum.unity.com/threads/audio-mixer-linear-volume.351864/
    private float ConvertToDB(float value) => Mathf.Clamp(Mathf.Log10(value) * 20, -80, 0);
    private float ConvertFromDB(float value) => Mathf.Pow(10, value / 20);

}
