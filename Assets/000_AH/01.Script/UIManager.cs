using Code.Core;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _stageText;

    [SerializeField] private GameObject _menuParent;

    [SerializeField] private Image _resetImage;

    [SerializeField]private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    public void OnPointerEnterRotate()
    {
        if (_resetImage != null)
        {
            _resetImage.rectTransform.DORotate(Vector3.zero, 0.1f);
            
            _resetImage.transform.DOKill();

            _resetImage.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);
        }
    }
    public void SetStageText(string stage)
    {
        _stageText.text = stage;
    }

    public void MenuClick()
    {

    }
    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        HashSet<string> options = new HashSet<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(new List<string>(options));
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
