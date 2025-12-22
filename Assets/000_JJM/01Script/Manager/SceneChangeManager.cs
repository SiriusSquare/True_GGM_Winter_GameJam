using Code.Core;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class SceneChangeManager : MonoSingleton<SceneChangeManager>
{
    private FadeManager fade;

    private void Awake()
    {
        fade = FadeManager.Instance;
    }
        
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(PlayFade(() =>
        {
            SceneManager.LoadScene(sceneName);
            fade.FadeOut();
        }));
    }

    public void ChangeScene(int sceneIndex)
    {
        Debug.Log("ChangeScene: " + sceneIndex);
        StartCoroutine(PlayFade(() =>
        {
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
