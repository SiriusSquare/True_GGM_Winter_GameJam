using DG.Tweening;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    private Vector3 originalScale;
    private RectTransform rect;

    [SerializeField] private float duration = 0.35f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool playOnEnable = true;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            originalScale = transform.localScale;
            return;
        }

        originalScale = rect.localScale;

        Vector2 prevAnchored = rect.anchoredPosition;
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = prevAnchored;

        rect.localScale = new Vector3(originalScale.x, 0f, originalScale.z);
    }

    private void OnEnable()
    {
        if (playOnEnable)
            PlayScaleUp();
    }
    
    [ContextMenu("PlayScaleUp")]
    public void PlayScaleUp()
    {
        if (rect == null)
        {
            transform.DOKill();
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            transform.DOScale(originalScale, duration).SetEase(ease);
            return;
        }

        rect.DOKill();
        rect.localScale = new Vector3(originalScale.x, 0f, originalScale.z);
        rect.DOScaleY(originalScale.y, duration).SetEase(ease).SetUpdate(true);
    }
}
