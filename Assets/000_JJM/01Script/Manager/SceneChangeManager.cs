using Code.Core;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class SceneChangeManager : MonoSingleton<SceneChangeManager>
{
    private FadeManager fade;

    protected override void Awake()
    {
        fade = FadeManager.Instance;
    }
        
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(PlayFade(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
            fade.FadeOut();
        }));
    }

    public void ChangeScene(int sceneIndex)
    {
        Debug.Log("ChangeScene: " + sceneIndex);
        StartCoroutine(PlayFade(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneIndex);
            fade.FadeOut();
        }));
    }

    private IEnumerator PlayFade(Action loadAction)
    {
        fade.FadeIn();
        yield return new WaitForSeconds(fade.ChangeTime);
        loadAction();
    }
}
