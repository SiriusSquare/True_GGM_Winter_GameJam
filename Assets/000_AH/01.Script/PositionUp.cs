using DG.Tweening;
using UnityEngine;

public class PositionUp : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 originalAnchoredPos;
    private Vector3 originalLocalPos;

    [SerializeField] private float riseDistance = 100f;
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private bool playOnEnable = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            originalAnchoredPos = rect.anchoredPosition;
            rect.anchoredPosition = originalAnchoredPos - new Vector2(0f, riseDistance);
        }
        else
        {
            originalLocalPos = transform.localPosition;
            transform.localPosition = originalLocalPos - new Vector3(0f, riseDistance, 0f);
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
            PlayRise();
    }

    [ContextMenu("PlayRise")]
    public void PlayRise()
    {
        if (rect != null)
        {
            rect.DOKill();
            rect.anchoredPosition = originalAnchoredPos - new Vector2(0f, riseDistance);
            rect.DOAnchorPos(originalAnchoredPos, duration).SetEase(ease).SetUpdate(true);
        }
        else
        {
            transform.DOKill();
            transform.localPosition = originalLocalPos - new Vector3(0f, riseDistance, 0f);
            transform.DOLocalMove(originalLocalPos, duration).SetEase(ease).SetUpdate(true);
        }
    }
}
