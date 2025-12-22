using Code.Core;
using UnityEngine;
using UnityEngine.Audio;

public class CoreScript : MonoSingleton<CoreScript>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    // ===== Settings =====
    public float MasterSetting
    {
        get => _master;
        set
        {
            _master = Mathf.Clamp01(value);
            ApplyMaster();
            PlayerPrefs.SetFloat("MasterSetting", _master);
        }
    }
    public float BGMSetting
    {
        get => _bgm;
        set
        {
            _bgm = Mathf.Clamp01(value);
            ApplyBGM();
            PlayerPrefs.SetFloat("BGMSetting", _bgm);
        }
    }

    public float SFXSetting
    {
        get => _sfx;
        set
        {
            _sfx = Mathf.Clamp01(value);
            ApplySFX();
            PlayerPrefs.SetFloat("SFXSetting", _sfx);
        }
    }


    public int ResolutionSetting
    {
        get => _resolutionIndex;
        set
        {
            _resolutionIndex = value;
            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
        }
    }

    public bool isPause = false;

    // ===== Backing Fields =====
    private float _master;
    private float _bgm;
    private float _sfx;
    private int _resolutionIndex;

    protected override void Awake()
    {
        base.Awake();

        _bgm = PlayerPrefs.GetFloat("BGMSetting", 0.5f);
        _sfx = PlayerPrefs.GetFloat("SFXSetting", 0.5f);
        _master = PlayerPrefs.GetFloat("MasterSetting", 0.5f);
        _resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);

        ApplyAll();
    }

    // ===== Apply =====

    private void ApplyAll()
    {
        ApplyBGM();
        ApplySFX();
    }
    private void ApplyMaster()
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb(_master));
    }

    private void ApplyBGM()
    {
        audioMixer.SetFloat("BGMVolume", LinearToDb(_bgm));
    }

    private void ApplySFX()
    {
        audioMixer.SetFloat("SFXVolume", LinearToDb(_sfx));
    }
    // ===== Utils =====

    private float LinearToDb(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioMixer == null) return;

        ApplyAll();
    }
#endif

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
