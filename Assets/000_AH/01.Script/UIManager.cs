using Code.Core;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _stageText;

    [SerializeField] private GameObject _menuParent;

    [SerializeField] private Image _resetImage;

    private Button _resetBtn;

    [SerializeField] private SettingManager _settingManager;

    [SerializeField] private TextMeshProUGUI _gameStartText;



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
        if (_gameStartText != null)
        {
            _gameStartText.transform.DOScale(1, 0.1f);
        }
        if (_resetImage != null)
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
        if (_resetBtn != null)
        {
            _resetBtn.onClick.AddListener(Reset123);
        }

    }
    private IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (AH_StageManager.Instance != null && AH_StageManager.Instance.stages != null)
            {
                string currentName = AH_StageManager.Instance.GetCurrentStageName();
                SetStageText(currentName);
            }
            else
            {
                SetStageText(SceneManager.GetActiveScene().name);
            }
        }
        if (_gameStartText != null)
        {
            yield return new WaitForSeconds(1);
            _gameStartText.gameObject.SetActive(true);
            _gameStartText.DOColor(new Color(1,1,1,0), 1f).SetUpdate(true);
        }
    }
    protected override void OnDestroy()
    {
        if (_resetBtn != null)
        {
            _resetBtn.onClick.RemoveListener(Reset123);
        }
    }
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if(_resetImage.rectTransform.rotation != Quaternion.Euler(Vector3.zero))
                _resetImage.rectTransform.DORotate(Vector3.zero, 0.1f);
            _resetBtn.interactable = false;
        }
        else
        {
            if (_resetBtn != null && !_resetBtn.interactable)
                _resetBtn.interactable = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuClick();
        }
    }

    public void Reset123()
    {
        _resetBtn.interactable = false;
        StartCoroutine(ResetBtn());
    }
    public IEnumerator ResetBtn()
    {
        yield return new WaitForSeconds(1f);
        SceneChangeManager.Instance.ChangeScene(SceneManager.GetActiveScene().buildIndex);
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
    public void GameStart()
    {
        SceneChangeManager.Instance.ChangeScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void SetStageText(string stage)
    {
        if(_stageText != null)
            _stageText.text = stage;
        if(_gameStartText != null)
            _gameStartText.text = stage;
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
            if (_settingManager != null && _settingManager.gameObject.activeSelf)
            {
                _settingManager.CloseSetting();
                return;
            }
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
        Time.timeScale = 1f;
        SceneChangeManager.Instance.ChangeScene("MainMenu");
    }
}
