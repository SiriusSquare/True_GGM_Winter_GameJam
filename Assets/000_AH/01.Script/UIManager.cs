using Code.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _stageText;

    [SerializeField] private GameObject _menuParent;

    [SerializeField] private Image _resetImage;

    [SerializeField] private Button _resetBtn;

    [SerializeField] private SettingManager _settingManager;


    [Header("Menu Animation")]
    [SerializeField] private Vector2 _menuVisibleAnchoredPos = Vector2.zero;
    [SerializeField] private Vector2 _menuHiddenAnchoredPos = new Vector2(0f, -600f);
    [SerializeField] private float _menuAnimDuration = 0.35f;
    [SerializeField] private Ease _menuOpenEase = Ease.OutCubic;
    [SerializeField] private Ease _menuCloseEase = Ease.InCubic;

    private RectTransform _menuRect;
    private Tween _menuTween;

    protected override void Awake()
    {
        base.Awake();
        _resetBtn = _resetImage.GetComponent<Button>();
        if (_menuParent != null)
        {
            _menuRect = _menuParent.GetComponent<RectTransform>();
            if (_menuRect != null)
            {
                _menuRect.anchoredPosition = _menuHiddenAnchoredPos;
            }
            _menuParent.SetActive(false);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if(_resetImage.rectTransform.rotation != Quaternion.Euler(Vector3.zero))
                _resetImage.rectTransform.DORotate(Vector3.zero, 0.1f);
            _resetBtn.interactable = false;
            return;
        }
        else
        {
            if (_resetBtn != null && !_resetBtn.interactable)
                _resetBtn.interactable = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_settingManager != null && _settingManager.gameObject.activeSelf)
            {
                _settingManager.CloseSetting();
            }
            MenuClick();
        }
    }
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
        if (_menuParent == null) return;

        bool willOpen = !_menuParent.activeSelf;

        if (willOpen)
        {
            Time.timeScale = 0f;

            _menuParent.SetActive(true);

            if (_menuRect != null)
            {
                _menuTween?.Kill();
                _menuRect.anchoredPosition = _menuHiddenAnchoredPos;
                _menuTween = _menuRect.DOAnchorPos(_menuVisibleAnchoredPos, _menuAnimDuration)
                    .SetEase(_menuOpenEase)
                    .SetUpdate(true);
            }
        }
        else
        {
            Time.timeScale = 1f;

            if (_menuRect != null)
            {
                _menuTween?.Kill();
                _menuTween = _menuRect.DOAnchorPos(_menuHiddenAnchoredPos, _menuAnimDuration)
                    .SetEase(_menuCloseEase)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        if (_menuParent != null)
                            _menuParent.SetActive(false);
                    });
            }
            else
            {
                _menuParent.SetActive(false);
            }
        }
    }

    public void GotoMainScene()
    {
        SceneChangeManager.Instance.ChangeScene("MainMenu");
    }
}
