using Code.Core;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _stageText;

    public void SetStageText(string stage)
    {
        _stageText.text = stage;
    }

}
