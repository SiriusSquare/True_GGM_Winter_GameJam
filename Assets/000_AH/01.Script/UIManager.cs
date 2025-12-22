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

    public void OnPointerEnterRotate()
    {
        if (_resetImage != null)
        {
            _resetImage.rectTransform.DORotate(Vector3.zero, 0.1f);
            
            _resetImage.transform.DOKill();

            _resetImage.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic);
        }
    }
    public void SetStageText(string stage)
    {
        _stageText.text = stage;
    }

    public void MenuClick()
    {

    }
}
