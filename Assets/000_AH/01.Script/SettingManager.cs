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

    #region Resolution Settings
    [SerializeField]private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        gameObject.SetActive(false);

        var allRes = Screen.resolutions;
        var uniqueList = new List<Resolution>();
        var options = new List<string>();

        for (int i = 0; i < allRes.Length; i++)
        {
            var r = allRes[i];
            if (r.width < 640 || r.width > 1920 || r.height < 540 || r.height > 1080)
                continue;

            bool exists = false;
            for (int j = 0; j < uniqueList.Count; j++)
            {
                if (uniqueList[j].width == r.width && uniqueList[j].height == r.height)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                uniqueList.Add(r);
            }
        }

        uniqueList.Sort((a, b) =>
        {
            int w = b.width.CompareTo(a.width);
            return w != 0 ? w : b.height.CompareTo(a.height);
        });

        for (int i = 0; i < uniqueList.Count; i++)
        {
            options.Add(uniqueList[i].width + " x " + uniqueList[i].height);
        }

        resolutions = uniqueList.ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        int savedIndex = CoreScript.Instance != null ? CoreScript.Instance.ResolutionSetting : 0;
        int chosenIndex = 0;

        if (CoreScript.Instance != null && savedIndex >= 0 && savedIndex < resolutions.Length)
        {
            chosenIndex = savedIndex;
        }
        else if (CoreScript.Instance != null)
        {
            if (savedIndex >= 0 && savedIndex < allRes.Length)
            {
                var savedRes = allRes[savedIndex];
                for (int i = 0; i < resolutions.Length; i++)
                {
                    if (resolutions[i].width == savedRes.width && resolutions[i].height == savedRes.height)
                    {
                        chosenIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < resolutions.Length; i++)
                {
                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        chosenIndex = i;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    chosenIndex = i;
                    break;
                }
            }
        }

        resolutionDropdown.SetValueWithoutNotify(savedIndex);
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        if (CoreScript.Instance != null)
        {
            if (_masterSlider != null)
            {
                _masterSlider.SetValueWithoutNotify(CoreScript.Instance.MasterSetting);
                UpdateValueText(_masterValueText, CoreScript.Instance.MasterSetting);
                _masterSlider.onValueChanged.AddListener(v =>
                {
                    CoreScript.Instance.MasterSetting = v;
                    UpdateValueText(_masterValueText, v);
                });
            }

            if (_bgmSlider != null)
            {
                _bgmSlider.SetValueWithoutNotify(CoreScript.Instance.BGMSetting);
                UpdateValueText(_bgmValueText, CoreScript.Instance.BGMSetting);
                _bgmSlider.onValueChanged.AddListener(v =>
                {
                    CoreScript.Instance.BGMSetting = v;
                    UpdateValueText(_bgmValueText, v);
                });
            }

            if (_sfxSlider != null)
            {
                _sfxSlider.SetValueWithoutNotify(CoreScript.Instance.SFXSetting);
                UpdateValueText(_sfxValueText, CoreScript.Instance.SFXSetting);
                _sfxSlider.onValueChanged.AddListener(v =>
                {
                    CoreScript.Instance.SFXSetting = v;
                    UpdateValueText(_sfxValueText, v);
                });
            }
        }
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
        gameObject.SetActive(true);
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
}
