using _JJM.Script.CustomEditor;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonOnMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [ReadOnly, SerializeField] private RectTransform _transform;

    private Vector3 _saveSize;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _saveSize = _transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _transform.DOScale(_saveSize * 1.1f, 0.1f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _transform.DOScale(_saveSize, 0.1f).SetUpdate(true);
    }
}
