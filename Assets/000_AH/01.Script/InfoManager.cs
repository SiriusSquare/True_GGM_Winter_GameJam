using Code.Core;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Info
{
    public InfoSO InfoSO;
    public bool IsSceneShow;
}

[System.Serializable]
public class InfoData
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;
    public Image image;
}
public class InfoManager : MonoSingleton<InfoManager>
{
    [Header("Slide Animation")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Vector2 visibleAnchoredPos = Vector2.zero;
    [SerializeField] private Vector2 hiddenAnchoredPos = new Vector2(0f, -600f);
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private Ease openEase = Ease.OutCubic;
    [SerializeField] private Ease closeEase = Ease.InCubic;

    [SerializeField] private Sprite lockedSprite;

    [Header("Many")]
    [SerializeField] private InfoData[] infoData;
    
    private Tween activeTween;
    private void Start()
    {
        gameObject.SetActive(false);
        Initialize();
    }
    private void Initialize()
    {
        if(AH_StageManager.Instance == null)
        {
            Debug.LogWarning("AH_StageManager 인스턴스가 없습니다.");
            return;
        }

        for(int i = 0; i < infoData.Length; i++)
        {
            if (infoData[i].TitleText != null)
            {
                infoData[i].TitleText.text = AH_StageManager.Instance.GetCurrentStageData().infos[i].InfoSO.Title;
            }
            if (infoData[i].DescriptionText != null)
            {
                infoData[i].DescriptionText.text = AH_StageManager.Instance.GetCurrentStageData().infos[i].InfoSO.Description;
            }
            if(infoData[i].image != null)
            {
                if (AH_StageManager.Instance.GetCurrentStageData().infos[i].IsSceneShow)
                {
                    infoData[i].image.sprite = AH_StageManager.Instance.GetCurrentStageData().infos[i].InfoSO.Image;
                }
                else
                {
                    infoData[i].image.sprite = lockedSprite;
                    infoData[i].TitleText.text = "????";
                    infoData[i].DescriptionText.text = "잠금 해제 조건을 달성해야 볼 수 있습니다.";
                }
            }
        }
    }
    public void OpenInfo()
    {
        bool willOpen = !gameObject.activeSelf;

        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogWarning("RectTransform이 없습니다.");
                return;
            }
        }
        if (willOpen)
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);

            activeTween?.Kill();

            panelRect.anchoredPosition = hiddenAnchoredPos;
            activeTween = panelRect.DOAnchorPos(visibleAnchoredPos, animationDuration)
                .SetEase(openEase)
                .SetUpdate(true);
        }
        else
        {
            Time.timeScale = 1f;
            CloseInfo();
        }
    }
    public void CloseInfo()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogWarning("RectTransform이 없습니다.");
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(false);
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
}
