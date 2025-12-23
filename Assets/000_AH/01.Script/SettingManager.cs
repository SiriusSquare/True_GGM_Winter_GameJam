using Code.Core;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoSingleton<SettingManager>
{
    [Header("Slide Animation")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 visibleAnchoredPos = Vector2.zero;
    [SerializeField] private Vector2 hiddenAnchoredPos = new Vector2(0f, -600f);
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private Ease openEase = Ease.OutCubic;
    [SerializeField] private Ease closeEase = Ease.InCubic;

    private Tween activeTween;

    [Header("Volume Settings")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private TextMeshProUGUI _masterValueText;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private TextMeshProUGUI _bgmValueText;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _sfxValueText;

    [Header("Mute")]
    [SerializeField] private Toggle _masterMuteToggle;
    [SerializeField] private Toggle _bgmMuteToggle;
    [SerializeField] private Toggle _sfxMuteToggle;
    [SerializeField] private float _zeroDownDuration = 0.2f;

    [Header("SoundSprites")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _muteSprite;
    [SerializeField] private Sprite _soundSmall;
    [SerializeField] private Sprite _soundMedium;
    [SerializeField] private Sprite _soundLarge;

    [Header("DisplayMode")]
    [SerializeField] private TMP_Dropdown _displaymode;
    public enum ScreenMode
    {
        FullScreenWindow,
        Window
    }

    [Header("Resolution Settings")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    private Resolution[] resolutions;

    private float _lastMasterVol = 1f;
    private float _lastBgmVol = 1f;
    private float _lastSfxVol = 1f;

    private bool _isMasterFading = false;
    private bool _isBgmFading = false;
    private bool _isSfxFading = false;

    private void Start()
    {
        List<string> options = new List<string> {
            "전체화면",
            "창모드"
        };

        _displaymode.ClearOptions();
        _displaymode.AddOptions(options);
        _displaymode.value = Screen.fullScreenMode == FullScreenMode.Windowed ? 1 : 0;
        _displaymode.RefreshShownValue();

        _displaymode.onValueChanged.AddListener(index => ChangeFullScreenMode((ScreenMode)index));
        gameObject.SetActive(false);
        InitResolution();
        InitSound();
    }
    private void ChangeFullScreenMode(ScreenMode mode)
    {
        int width = Screen.width;
        int height = Screen.height;

        if (resolutions != null && _resolutionDropdown.value < resolutions.Length)
        {
            width = resolutions[_resolutionDropdown.value].width;
            height = resolutions[_resolutionDropdown.value].height;
        }

        switch (mode)
        {
            case ScreenMode.FullScreenWindow:
                Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                break;
            case ScreenMode.Window:
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                break;
        }
    }
    private void InitSound()
    {
        if (CoreScript.Instance == null) return;

        if (_masterSlider != null && _masterMuteToggle != null)
        {
            float savedVol = CoreScript.Instance.MasterSetting;
            _lastMasterVol = savedVol > 0.001f ? savedVol : 1f;

            _masterSlider.SetValueWithoutNotify(savedVol);
            UpdateValueText(_masterValueText, savedVol);

            bool isMuted = savedVol <= 0.001f;
            _masterMuteToggle.SetIsOnWithoutNotify(isMuted);
            UpdateToggleIcon(_masterMuteToggle, isMuted ? 0 : savedVol);

            CoreScript.Instance.MasterSetting = savedVol;

            _masterSlider.onValueChanged.AddListener(v =>
            {
                UpdateValueText(_masterValueText, v);
                CoreScript.Instance.MasterSetting = v;
                UpdateToggleIcon(_masterMuteToggle, v);

                if (_masterMuteToggle.isOn && !_isMasterFading && v > 0.001f)
                {
                    _masterMuteToggle.SetIsOnWithoutNotify(false);
                }
            });

            _masterMuteToggle.onValueChanged.AddListener(isOn =>
            {
                _masterSlider.DOKill(); 
                _isMasterFading = true; 

                if (isOn)
                {
                    if (_masterSlider.value > 0.001f) _lastMasterVol = _masterSlider.value;

                    _masterSlider.DOValue(0f, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isMasterFading = false;
                            CoreScript.Instance.MasterSetting = 0f;
                        });
                    UpdateToggleIcon(_masterMuteToggle, 0f);
                }
                else
                {
                    float targetVol = _lastMasterVol > 0.001f ? _lastMasterVol : 1f;

                    _masterSlider.DOValue(targetVol, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isMasterFading = false;
                            CoreScript.Instance.MasterSetting = targetVol;
                        });
                    UpdateToggleIcon(_masterMuteToggle, targetVol);
                }
            });
        }

        if (_bgmSlider != null && _bgmMuteToggle != null)
        {
            float savedVol = CoreScript.Instance.BGMSetting;
            _lastBgmVol = savedVol > 0.001f ? savedVol : 1f;

            _bgmSlider.SetValueWithoutNotify(savedVol);
            UpdateValueText(_bgmValueText, savedVol);

            bool isMuted = savedVol <= 0.001f;
            _bgmMuteToggle.SetIsOnWithoutNotify(isMuted);
            UpdateToggleIcon(_bgmMuteToggle, isMuted ? 0 : savedVol);

            CoreScript.Instance.BGMSetting = savedVol;

            _bgmSlider.onValueChanged.AddListener(v =>
            {
                UpdateValueText(_bgmValueText, v);
                CoreScript.Instance.BGMSetting = v;
                UpdateToggleIcon(_bgmMuteToggle, v);

                if (_bgmMuteToggle.isOn && !_isBgmFading && v > 0.001f)
                {
                    _bgmMuteToggle.SetIsOnWithoutNotify(false);
                }
            });

            _bgmMuteToggle.onValueChanged.AddListener(isOn =>
            {
                _bgmSlider.DOKill();
                _isBgmFading = true;

                if (isOn)
                {
                    if (_bgmSlider.value > 0.001f) _lastBgmVol = _bgmSlider.value;

                    _bgmSlider.DOValue(0f, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isBgmFading = false;
                            CoreScript.Instance.BGMSetting = 0f;
                        });
                    UpdateToggleIcon(_bgmMuteToggle, 0f);
                }
                else
                {
                    float targetVol = _lastBgmVol > 0.001f ? _lastBgmVol : 1f;

                    _bgmSlider.DOValue(targetVol, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isBgmFading = false;
                            CoreScript.Instance.BGMSetting = targetVol;
                        });
                    UpdateToggleIcon(_bgmMuteToggle, targetVol);
                }
            });
        }

        if (_sfxSlider != null && _sfxMuteToggle != null)
        {
            float savedVol = CoreScript.Instance.SFXSetting;
            _lastSfxVol = savedVol > 0.001f ? savedVol : 1f;

            _sfxSlider.SetValueWithoutNotify(savedVol);
            UpdateValueText(_sfxValueText, savedVol);

            bool isMuted = savedVol <= 0.001f;
            _sfxMuteToggle.SetIsOnWithoutNotify(isMuted);
            UpdateToggleIcon(_sfxMuteToggle, isMuted ? 0 : savedVol);

            CoreScript.Instance.SFXSetting = savedVol;

            _sfxSlider.onValueChanged.AddListener(v =>
            {
                UpdateValueText(_sfxValueText, v);
                CoreScript.Instance.SFXSetting = v;
                UpdateToggleIcon(_sfxMuteToggle, v);

                if (_sfxMuteToggle.isOn && !_isSfxFading && v > 0.001f)
                {
                    _sfxMuteToggle.SetIsOnWithoutNotify(false);
                }
            });

            _sfxMuteToggle.onValueChanged.AddListener(isOn =>
            {
                _sfxSlider.DOKill();
                _isSfxFading = true;

                if (isOn)
                {
                    if (_sfxSlider.value > 0.001f) _lastSfxVol = _sfxSlider.value;

                    _sfxSlider.DOValue(0f, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isSfxFading = false;
                            CoreScript.Instance.SFXSetting = 0f;
                        });
                    UpdateToggleIcon(_sfxMuteToggle, 0f);
                }
                else
                {
                    float targetVol = _lastSfxVol > 0.001f ? _lastSfxVol : 1f;

                    _sfxSlider.DOValue(targetVol, _zeroDownDuration)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            _isSfxFading = false;
                            CoreScript.Instance.SFXSetting = targetVol;
                        });
                    UpdateToggleIcon(_sfxMuteToggle, targetVol);
                }
            });
        }
    }


    #region Resolution
    private void InitResolution()
    {
        var allRes = Screen.resolutions;
        var uniqueList = new List<Resolution>();
        var options = new List<string>();

        for (int i = 0; i < allRes.Length; i++)
        {
            var r = allRes[i];
            if (r.width < 640 || r.width > 1920 || r.height < 540 || r.height > 1080) continue;

            bool exists = false;
            for (int j = 0; j < uniqueList.Count; j++)
            {
                if (uniqueList[j].width == r.width && uniqueList[j].height == r.height)
                {
                    exists = true; break;
                }
            }
            if (!exists) uniqueList.Add(r);
        }

        uniqueList.Sort((a, b) =>
        {
            int w = b.width.CompareTo(a.width);
            return w != 0 ? w : b.height.CompareTo(a.height);
        });

        for (int i = 0; i < uniqueList.Count; i++) options.Add(uniqueList[i].width + " x " + uniqueList[i].height);

        resolutions = uniqueList.ToArray();
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);

        int savedIndex = CoreScript.Instance != null ? CoreScript.Instance.ResolutionSetting : 0;
        _resolutionDropdown.SetValueWithoutNotify(savedIndex);
        _resolutionDropdown.RefreshShownValue();
        _resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        if (CoreScript.Instance != null)
        {
            CoreScript.Instance.ResolutionSetting = resolutionIndex;
        }
    }
    #endregion

    public void OpenSetting()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(5);
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogWarning("RectTransform이 없습니다.");
                return;
            }
        }

        gameObject.SetActive(true);

        activeTween?.Kill();

        panelRect.anchoredPosition = hiddenAnchoredPos;
        activeTween = panelRect.DOAnchorPos(visibleAnchoredPos, animationDuration)
            .SetEase(openEase)
            .SetUpdate(true);
    }
    public void CloseSetting()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogWarning("RectTransform이 없습니다.");
                gameObject.SetActive(false);
                return;
            }
        }

        activeTween?.Kill();

        activeTween = panelRect.DOAnchorPos(hiddenAnchoredPos, animationDuration)
            .SetEase(closeEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void UpdateValueText(TextMeshProUGUI text, float value)
    {
        if (text == null) return;
        int percent = Mathf.RoundToInt(value * 100f);
        text.text = percent + "%";
    }
    private void UpdateToggleIcon(Toggle toggle, float value)
    {
        if (toggle == null) return;

        Image targetImage = toggle.targetGraphic as Image;

        if (targetImage == null)
        {
            targetImage = toggle.GetComponentInChildren<Image>();
        }

        if (targetImage == null)
        {
            Debug.LogWarning($"Toggle [{toggle.name}]에 변경할 Image(Target Graphic)가 없습니다.");
            return;
        }

        if (toggle.isOn || value <= 0.001f)
        {

            targetImage.sprite = _muteSprite != null ? _muteSprite : _defaultSprite;
        }
        else
        {
            if (value < 0.33f)
            {
                targetImage.sprite = _soundSmall != null ? _soundSmall : _defaultSprite;
            }
            else if (value < 0.66f)
            {
                targetImage.sprite = _soundMedium != null ? _soundMedium : _defaultSprite;
            }
            else
            {
                targetImage.sprite = _soundLarge != null ? _soundLarge : _defaultSprite;
            }
        }
    }
}