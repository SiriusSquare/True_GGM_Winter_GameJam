using UnityEngine;
using Code.Core;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoSingleton<FadeManager>
{
    [SerializeField] private Canvas _canva;
    [SerializeField] private Image _image;
    [field: SerializeField] public float ChangeTime { get; private set; } = 0.2f;

    private Sequence _seq;

    protected override void Awake()
    {   
        base.Awake();
        if(SceneManager.GetActiveScene().buildIndex != 0)
            DontDestroyOnLoad(gameObject);
        FadeOut();
    }

    public void FadeOut()
    {
        _canva.sortingOrder = 100;
        Debug.Log("¾Æ¿ô");
        _seq = DOTween.Sequence().SetUpdate(true);
        _seq.Append(_image.DOFade(1, 0));
        _seq.Append(_image.DOFade(0, ChangeTime));
        _seq.AppendCallback(() =>
        {
            _canva.sortingOrder = -100;
        });

    }
    public void FadeIn()
    {
        _seq = DOTween.Sequence().SetUpdate(true);
        _seq.Append(_image.DOFade(0, 0));
        _seq.AppendCallback(() =>
        {
            _canva.sortingOrder = 100;
        });
        _seq.Append(_image.DOFade(1, ChangeTime));
    }
}
